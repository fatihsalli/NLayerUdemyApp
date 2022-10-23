using NLayer.Core.DTOs;
using NLayer.Core.Models;

namespace NLayer.Core.Services
{
    public interface ICategoryService : IService<Category>
    {
        Task<CustomResponseDto<CategoryWithProductsDto>> GetSingleCategoryByIdWithProductsAsync(int categoryId);
    }
}
