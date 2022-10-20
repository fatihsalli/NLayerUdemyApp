using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    //"api/[controller]/[action] tanımlamasını neden yapmadık? Zaten Get-Post vb. ayırdık. Id'ye göre bulmakistediği taktirde ise GET=> www.mysite.com/api/products/5 şeklinde istek yapılacak."
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CustomBaseController
    {
        //Controllerlarda repository değil serviceleri implemente etmek gerekir.
        private readonly IMapper _mapper;
        private readonly IService<Product> _service;
        private readonly IProductService _productService;
        public ProductsController(IMapper mapper, IService<Product> service, IProductService productService)
        {
            _mapper = mapper;
            _service = service;
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
            var products = await _service.GetAllAsync();
            //Automapper kullanarak mapleme yani transfer işlemini gerçekleştirdik.
            var productsDto = _mapper.Map<List<ProductDto>>(products.ToList());
            //Her seferinde hem Ok vs. yazmaktansa CustomBaseControllerda CreateActionResult methodu tanımladık.
            //Soluk bir ifade var ise bunu belirtmeye gerek olmadığını ifade eder.
            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, productsDto));
        }

        // GET=> www.mysite.com/api/products/5 => bu 5 'i görmek için HttpGet içinde tanımlıyoruz.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAync(id);
            var productDto= _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productDto));
        }

        //Create de geriye değer dönüyoruz çünkü Client id'yi görmek ister. Id'de database'e kaydedildiği anda oluşturulduğu için geriye Response code ile birlikte değer dönerek id'yi göndermiş oluyoruz.
        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            var product = await _service.AddAsync(_mapper.Map<Product>(productDto));
            //Client'a response dönebilmek için tekrar mapleyerek productsDto'yu geriye döndük.
            var productsDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, productsDto));
        }

        //ProductUpdateDto olarak özel bir sınıf oluşturmuştuk. Bunun sebebi de update işleminde CreatedDate'e gerek olmadığı için.
        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto productDto)
        {
            await _service.UpdateAsync(_mapper.Map<Product>(productDto));
            //Geriye değer döndürmediğimiz için Success'in diğer methodunu kullandık. NoContentDto ile birlikte.
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

        // DELETE => www.mysite.com/api/products/5 - Id'si olan 5 ürün silinir. eğer ki id'yi buraya tanımlamasaydık www.mysite.com/api/products?id=5 olarak yazacaktı. Best practise açısından uygun değildir.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _service.GetByIdAync(id);
            await _service.RemoveAsync(product);
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }






    }
}
