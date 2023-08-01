using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NLayer.API.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    //"api/[controller]/[action] tanımlamasını neden yapmadık? Zaten Get-Post vb. ayırdık. Id'ye göre bulmak istediği taktirde ise GET=> www.mysite.com/api/products/5 şeklinde istek yapılacak."
    public class ProductsController : CustomBaseController
    {
        //Controllerlarda repository değil serviceleri implemente etmek gerekir.
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductsController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        // GET=> www.mysite.com/api/products/GetProductsWithCategory
        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductsWithCategory()
        {
            return CreateActionResult(await _productService.GetProductsWithCategoryAsync());
        }

        // GET=> www.mysite.com/api/products
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var products = await _productService.GetAllAsync();
            //Automapper kullanarak mapleme yani transfer işlemini gerçekleştirdik.
            var productsDto = _mapper.Map<List<ProductDto>>(products.ToList());
            //Her seferinde hem Ok vs. yazmaktansa CustomBaseControllerda CreateActionResult methodu tanımladık.
            //Soluk bir ifade var ise bunu belirtmeye gerek olmadığını ifade eder.
            //return Ok(CustomResponseDto<List<ProductDto>>.Success(200, productsDto)); bunu yazmak yerine CreateActionResult adında bir metot ile Status Code'u kendimiz dönüyoruz.
            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, productsDto));
        }

        // GET=> www.mysite.com/api/products/5 => bu 5 'i görmek için HttpGet içinde tanımlıyoruz.
        //Oluşturduğumuz NotFoundFilter'ı direkt olarak [NotFoundFilter] şeklinde ValidationFilter'daki gibi yazamayız. Bunun iki sebebi vardır."ValidateFilterAttribute" "ActionFilterAttribute" miras alır, "NotFoundFilter" ise "IAsyncActionFilter" dan miras aldığı için [Attribute] mirası yoktur. Neden IAsyncActionFilter seçtik? 1-Dinamik kullanmak için (Generic durum var) 2-Constructorda parametre geçtiğim için Attribute ve filterları [NotFoundFilter] şeklinde direkt yazamam aşağıdaki gibi yazmam gerekiyor.
        //Filter ile sistemde olmayan bir Id request edildiğinde actiona girmeden NotFound olarak döner. GetById içerisinde yazdığımız extensionda ise action içine girerek Id'nin database de olmadığını anladığında geriye Notfound döner fark budur.
        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productDto));
        }

        //Create de geriye değer dönüyoruz çünkü Client id'yi görmek ister. Id'de database'e kaydedildiği anda oluşturulduğu için geriye Response code ile birlikte değer dönerek id'yi göndermiş oluyoruz.
        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            var product = await _productService.AddAsync(_mapper.Map<Product>(productDto));
            //Client'a response dönebilmek için tekrar mapleyerek productsDto'yu geriye döndük. Oluşturulan ürünün Id'sini görmek isteyebilir.
            var productsDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, productsDto));
        }

        //ProductUpdateDto olarak özel bir sınıf oluşturmuştuk. Bunun sebebi de update işleminde CreatedDate'e gerek olmadığı için.
        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            //CreatedDate sorunu yaşadığım için bu 2 satır kod düzenlendi daha sonra kontrol edilecek. Sorunu Fatih Hoca çözdü. ProductRepository-GetProductsWithCategoryAsync methodunda Cachelemeden önce çektiğimiz dataların track özelliğini kapatmamız gerekiyor. Yoksa aynı id de 2 farklı model olduğu için hata alıyoruz.
            //var productCreatedDate = await _productService.GetByIdAsync(product.Id);
            //product.CreatedDate=productCreatedDate.CreatedDate;
            await _productService.UpdateAsync(product);
            //Geriye değer döndürmediğimiz için Success'in diğer methodunu kullandık. NoContentDto ile birlikte.
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

        // DELETE => www.mysite.com/api/products/5 - Id'si olan 5 ürün silinir. eğer ki id'yi buraya tanımlamasaydık www.mysite.com/api/products?id=5 olarak yazacaktı. Best practise açısından uygun değildir.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            await _productService.RemoveAsync(product);
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

        // Github fork test => deneme 1 - deneme 2 - deneme 3
    }
}
