using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core.Models
{
    public class Product : BaseEntity
    {
        //Constructor tetiklendiğinde name gelmez ise hata fırlatması için aşağıdaki fonksiyon yazılabilir. Name alanının null gelmemesi için oluşturulmuştur.
        //public Product(string name)
        //{
        //    Name = name ?? throw new ArgumentNullException(nameof(name));
        //}

        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        //[ForeignKey("CategoryId")] EF Core direkt görüyor eğer görmeseydi Attribute tanımlayacaktık.
        public Category Category { get; set; }

        //Aşağıdaki tanımlama "Navigation Property" olarak geçmektedir. Product'dan ProductFeature'a veya Category'e gidebilirim.
        public ProductFeature ProductFeature { get; set; }


    }
}
