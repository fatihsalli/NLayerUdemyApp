using AutoMapper;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Mapping
{
    public class MapProfile:Profile
    {
        public MapProfile()
        {
            //CreateMap ile önce source sonra destination seçilerek tanımlama yapıyoruz. ReverseMap ile çift taraflı yani destination=>source çevirebilmek için kullanıyoruz.
            CreateMap<Product, ProductDto>().ReverseMap();            
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<ProductFeature, ProductFeatureDto>().ReverseMap();
            //Tersine yani Product=> ProductUpdateDto'ya ihtiyacımız olmadığı için ReverseMap kullanmadık.
            CreateMap<ProductUpdateDto, Product>();

        }



    }
}
