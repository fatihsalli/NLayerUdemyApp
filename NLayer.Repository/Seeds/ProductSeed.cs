using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Seeds
{
    internal class ProductSeed : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(
                new Product { Id = 1, Name = "Faber Castell Pen", CategoryId = 1, Price = 100, Stock = 20, CreatedDate = DateTime.Now },
                new Product { Id = 2, Name = "Lamy Pen", CategoryId = 1, Price = 200, Stock = 20, CreatedDate = DateTime.Now },
                new Product { Id = 3, Name = "Cross Pen", CategoryId = 1, Price = 200, Stock = 40, CreatedDate = DateTime.Now },
                new Product { Id = 4, Name = "Animal Farm", CategoryId = 2, Price = 30, Stock = 53, CreatedDate = DateTime.Now },
                new Product { Id = 5, Name = "How To Be Right", CategoryId = 2, Price = 42, Stock = 28, CreatedDate = DateTime.Now },
                new Product { Id = 6, Name = "Mabbels Notebook Batman", CategoryId = 3, Price = 24, Stock = 82, CreatedDate = DateTime.Now }
                );
        }
    }
}
