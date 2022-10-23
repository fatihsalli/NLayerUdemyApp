using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using NLayer.Core.Repositories;

namespace NLayer.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category> GetSingleCategoryByIdWithProductsAsync(int categoryId)
        {
            //Eager Loading - data çekilirken productlarında alınmasını sağladık.
            //Neden SingleOrDefault yaptık FirstOrDefault yapmadık; Where(x => x.Id == categoryId) FirstOrDefaultta bu koşulu sağlayan ilk datayı getirir, SingleOrDefaultta ise bu koşulu sağlayan birden çok data var ise hata fırlatır.
            return await _context.Categories.Include(x => x.Products).Where(x => x.Id == categoryId).SingleOrDefaultAsync();
        }
    }
}
