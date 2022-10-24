using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;
using NLayer.Web.Filters;
using NLayer.Web.Services;

namespace NLayer.Web.Controllers
{
    //MVC ile alakalı tarafı oluştururken Api yokmuş gibi düşünüyoruz.
    public class ProductsController : Controller
    {
        private readonly ProductApiService _productService;
        private readonly CategoryApiService _categoryService;
        public ProductsController(ProductApiService productService, CategoryApiService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var customResponse = await _productService.GetProductsWithCategoryAsync();
            return View(customResponse);
        }

        public async Task<IActionResult> Save()
        {
            var categoriesDto = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categoriesDto, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                await _productService.SaveAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            //Eksik doldurulması durumunda geriye döndürmek için aşağıda tekrar yazdık.
            var categoriesDto = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categoriesDto, "Id", "Name");
            return View(productDto);
        }

        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        public async Task<IActionResult> Update(int id)
        {
            var productDto=await _productService.GetByIdAsync(id);
            var categoriesDto = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categoriesDto, "Id", "Name",productDto.CategoryId);
            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateDto productDto)
        {
            if (ModelState.IsValid)
            {
                await _productService.UpdateAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            //Kullanıcı hatalı bir değer girdiği taktirde tekrar categorileri gönderiyoruzki ekrandaki dropdown list tekrar dolsun.
            var categoriesDto = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categoriesDto, "Id", "Name", productDto.CategoryId);
            return View(productDto);
        }

        public async Task<IActionResult> Remove(int id)
        {
            await _productService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }





    }
}
