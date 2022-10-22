using NLayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core.DTOs
{
    public class ProductWithCategoryDto:ProductDto
    {
        //Category ismi önemli yoksa map işlemi esnasında Product'a çevirirken isim aynı olmaz ise çeviremez. (Product içerisinde  public Category Category { get; set; })
        public CategoryDto Category { get; set; }


    }
}
