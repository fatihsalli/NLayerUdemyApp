using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core.DTOs
{
    //Client tarafında her request ve response için ayrı ayrı Dtolar tanımlanabilir. Örneğin ProductUpdateDto olarak tanımladığımız bu class CreatedDate içermeyerek ProductDto dan özelleştiriyoruz. Best Practice olarak request ve responseları ayrı ayrı tanımlamak daha doğru bir yaklaşımdır.
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
