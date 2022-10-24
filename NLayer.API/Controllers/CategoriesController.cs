using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    //Tüm Controllerlara tek tek yazmak yerinde globalde yazıyoruz."ValidateFilterAttribute" isimle oluşturduğumuz class ile request sonrası response vermeden araya giriyoruz.
    //[ValidateFilterAttribute]
    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            var categoriesDto=_mapper.Map<List<CategoryDto>>(categories.ToList());
            return CreateActionResult(CustomResponseDto<List<CategoryDto>>.Success(200, categoriesDto));
        }

        // GET=> www.mysite.com/api/categories/GetSingleCategoryByIdWithProducts/5
        //Framework mapleyebilmesi için methotda nasıl yazıldıysa o şekilde yazılmalıdır (categoryId).
        [HttpGet("[action]/{categoryId}")]
        public async Task<IActionResult> GetSingleCategoryByIdWithProducts(int categoryId)
        {
            return CreateActionResult(await _categoryService.GetSingleCategoryByIdWithProductsAsync(categoryId));
        }

        



    }
}
