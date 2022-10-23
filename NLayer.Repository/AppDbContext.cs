using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using System.Reflection;

namespace NLayer.Repository
{
    public class AppDbContext : DbContext
    {
        //Startupta veritabanı yolunu verebilmek için DbContextOptions oluşturuyoruz.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }

        //CreatedDate ve UpdatedDate bilgilerini client'dan bağımsız otomatik olarak yapabilmek için SaveChange metodunu burada eziyoruz. Ef Core Entity Database'e Save edilene kadar Tracking yani takip ediyor. Biz burada Tracking edilen entity'i SaveChange ile database'e kaydetmeden önce araya girerek CreatedDate ya da UpdatedDate bilgilerini gireceğiz. (SaveChangeAsync ve SaveChange için yaptık)
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entityReference)
                {
                    switch (item.State)
                    {
                        case EntityState.Added:
                            entityReference.CreatedDate = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            //Önemli! UpdatedDate'i güncellerken CreatedDate'i default bir değer atıyor bunu yapmaması için aşağıdaki gibi bir tanımalama yaparak şunu diyoruz CreatedDate'e dokunma UpdatedDate'i oluştur.
                            Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
                            entityReference.UpdatedDate = DateTime.Now;
                            break;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entityReference)
                {
                    switch (item.State)
                    {
                        case EntityState.Added:
                            entityReference.CreatedDate = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            Entry(entityReference).Property(x => x.CreatedDate).IsModified = false;
                            entityReference.UpdatedDate = DateTime.Now;
                            break;
                    }
                }
            }
            return base.SaveChanges();
        }

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
                Color = "Kırmızı",
                Height = 150,
                Width = 12,
                ProductId = 1
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
