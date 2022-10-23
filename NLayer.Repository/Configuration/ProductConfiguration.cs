using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core.Models;

namespace NLayer.Repository.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //builder.HasKey(x => x.Id);
            //builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Stock).IsRequired();
            //HasColumnType ile veritabanı tarafında hangi propery ile tutmak istediğimizi söylüyoruz. decimal (18,2) demek=> benim parasal değerim 18 karakter olacak ve virgülden sonra 2 karakter olabilir(virgülden önce 16 ve virgülden sonra 2).
            builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
            //EF Core bu ilişkiyi kuruyor ancak örnek olması açısından yazılmıştır.
            builder.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);

        }
    }
}
