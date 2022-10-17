using Microsoft.EntityFrameworkCore;
using NLayer.Core;
using NLayer.Repository.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository
{
    public class AppDbContext:DbContext
    {
        //Startupta veritabanı yolunu verebilmek için DbContextOptions oluşturuyoruz.
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Tüm Configration dosyalarını nasıl buluyor "ApplyConfigurationsFromAssembly" methodu ile "IEntityTypeConfiguration" miras alan Assemblyleri buluyor. Assembly.GetExecutingAssembly() demek de çalıştığımız klasörde ara demektir. Configurationların yanında Seedlerde "IEntityTypeConfiguration" miras aldığı için ekstra bir işleme gerek yoktur.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //Aşağıda tek tek yazılabilir ancak configuration dosyasının çok fazla olabileceğini düşündüğümüzde üstteki metot ile tamamını alabiliyoruz.
            //modelBuilder.ApplyConfiguration(new ProductConfiguration());

            //Örnek olması için class yerine burada oluşturuldu. Seed => Default data
            modelBuilder.Entity<ProductFeature>().HasData(new ProductFeature()
            {
                Id = 1,
                Color="Kırmızı",
                Height=150,
                Width=12,
                ProductId=1
            }, 
            new ProductFeature()
            {
                Id = 2,
                Color = "Siyah",
                Height = 125,
                Width = 16,
                ProductId = 2
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
