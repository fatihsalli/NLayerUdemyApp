using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;

namespace NLayer.API.Filters
{
    public class ValidateFilterAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //FluentValidation (ProductDtoValidator) daki hatalar direkt olarak ModelState'e yüklenir o sebeple ModelState.IsValid komutunu FluentValidation ile birlikte kullanabiliriz.
            if (!context.ModelState.IsValid)
            {
                var errors=context.ModelState.Values.SelectMany(x => x.Errors).Select(x=> x.ErrorMessage).ToList();
                context.Result=new BadRequestObjectResult(CustomResponseDto<NoContentDto>.Fail(400,errors));
            }
        }


    }
}
