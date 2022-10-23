using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Service.Exceptions;
using System.Linq.Expressions;

namespace NLayer.Caching
{
    //"Decorator design pattern" veya ona çok yakın olan "Proxy design pattern" implementasyonunu gerçekleştireceğiz.
    public class ProductServiceWithCaching : IProductService
    {
        //C#'ta "const" Kavramı Bir değişkenin değerinin program boyunca sabit olarak tutulması istendiğinde const (sabit) ifadesinden yararlanılır. Tanımlandığı satırda değeri atanmalıdır. 
        private const string CacheProductKey = "productsCache";
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductServiceWithCaching(IMapper mapper, IMemoryCache memoryCache, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _memoryCache = memoryCache;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;

            //Uygulama ayağa kalktığında cache de yok ise buraya girerek oluşturacak daha sonra tekrar girmeyecek.
            if (!_memoryCache.TryGetValue(CacheProductKey, out _))
            {
                //GetProductsWithCategoryAsync asenkron olduğu için "Result" ile senkrona çeviriyoruz.
                _memoryCache.Set(CacheProductKey, _productRepository.GetProductsWithCategoryAsync().Result);
            }

        }
        //Çok sık erişip çok fazla güncellemediğimiz data "Cache" yapılması daha sağlıklıdır.
        public async Task<Product> AddAsync(Product entity)
        {
            await _productRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            //Database'e ekleme yaptıktan sonra tekrar "Cache" leme işlemini yapıyoruz.
            await CacheAllProductsAsync();
            return entity;

        }

        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _productRepository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entities;
        }

        //Metot içerisinde await kullanmadık ama Task olarak bir return dönmemiz gerekiyor o halde Task.FromResult() ile sonucun Task olarak dönmesini sağlayabiliriz.
        public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            return Task.FromResult(_memoryCache.Get<List<Product>>(CacheProductKey).Any(expression.Compile()));
        }


        //Productlar categoryleri ile birlikte service katmanına gidecek ancak ProductDto da category olmadığı için basılmayacak bu sebeple Client tarafına gönderilmeyecek. Tüm metotları aynı "Cache" i kullanarak yaptığımız için bu şekilde çözdük.
        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult(_memoryCache.Get<List<Product>>(CacheProductKey).AsEnumerable());
        }

        //Datayı "Cache" den aldığımız için asenkron yapmıyoruz. Metot dönüşü Task olduğu için Task.FromResult() metotunu kullandık.
        public Task<Product> GetByIdAsync(int id)
        {
            var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);
            if (product==null)
            {
                throw new NotFoundException($"{typeof(Product).Name}({id}) not found");
            }
            //return Task.FromResult(_memoryCache.Get<List<Product>>(CacheProductKey).Where(x => x.Id == id).SingleOrDefault());
            return Task.FromResult(product);
        }

        public Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategoryAsync()
        {
            var products=_memoryCache.Get<List<Product>>(CacheProductKey);
            var productsWithCategoryDto = _mapper.Map<List<ProductWithCategoryDto>>(products);
            return Task.FromResult(CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsWithCategoryDto));
        }

        public async Task RemoveAsync(Product entity)
        {
            _productRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _productRepository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _productRepository.Update(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            //Expression 'ı function'a çervirmek için "Compile" metotunu kullanıyoruz.
            return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }

        //Çağırdığımızda Cache lemek için burada bir metot tanımladık.
        public async Task CacheAllProductsAsync()
        {
            await _memoryCache.Set(CacheProductKey, _productRepository.GetProductsWithCategoryAsync());
        }


    }
}
