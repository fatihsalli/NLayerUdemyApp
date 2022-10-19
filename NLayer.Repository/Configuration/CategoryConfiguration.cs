using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NLayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Configuration
{
    //IEntityTypeConfiguration ile Context de yaptığımız işlemleri burada yapmamız daha uygun bir yaklaşımdır. Fluent Api yöntemiyle burada düzenleyebiliriz.
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            //builder.ToTable("Categories");
        }
    }
}
