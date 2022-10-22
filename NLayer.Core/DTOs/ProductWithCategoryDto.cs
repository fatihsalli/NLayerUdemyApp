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
        //Category ismi önemli yoksa bulamıyor.
        public CategoryDto Category { get; set; }


    }
}
