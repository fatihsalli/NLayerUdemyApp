using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using NLayer.Core.Repositories;

namespace NLayer.Repository.Repositories
{
    //"GenericRepository"'den miras alıyor böylece halihazırda yazdığımız kodları tekrar yazmadan kullanmış oluyoruz. Aksi halde sadece "IProductRepository" yazsa idik "IProductRepository", "IGenericRepository"'den miras aldığı için iki interfacedeki tüm fonksiyonları implemente etmemiz gerekiyordu.
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {

        }

        //GenericRepository'deki contexte erişebilmek için protected demiştik o sebeple miras aldığımız için bu fonksiyonda ulaşabiliyoruz.
        public async Task<List<Product>> GetProductsWithCategoryAsync()
        {
            //Eager Loading - data çekilirken categorylerinde alınmasını sağladık.
            //Lazy Loading - ihtiyaç halinde daha sonra çekilmesi durumu
            var products = await _context.Products.Include(x => x.Category).ToListAsync();
            return products;
        }
    }
}
