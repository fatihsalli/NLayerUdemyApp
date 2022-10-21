using FluentValidation;
using NLayer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Validations
{
    //Property üzerinde [Required(Error...)] yazmak yerine ayrı bir klasör "Validations" ve Product özelinde ProductDtoValidator adında bir class oluşturduk. "AbstractValidator" den miras aldırdık. Kendisi yüklediğimiz FluentValidation paketiyle gelmektedir.
    public class ProductDtoValidator:AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            //{PropertyName} bu tanımlama bize (x=> x.Name) deki Name'i verir.
            RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} is required");

            //Price,stok gibi değerler value tip oldukları için null olmayıp sıfır olarak gelir. Bu sebeple bu tip değerlerde NotNull,NotEmpty yerine aralık verilmelidir.
            RuleFor(x => x.Price).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
            RuleFor(x => x.Stock).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
            RuleFor(x => x.CategoryId).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");


        }

    }
}
