using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;

namespace NLayer.API.Filters
{
    //Hazır bir filter attribute'unden miras aldırıyoruz. Senkron yazdık, NotFoundFilterda asenkron yazıldı. Eğitim amaçlı...
    public class ValidateFilterAttribute:ActionFilterAttribute
    {
        //Neden Service katmanında yazmadık? MVC ve Api tarafındaki yazacağımız filterlar farklı olduğu için.
        //Burada override ediyoruz metot çalışırken,metot çalıştığında vs. gibi hangi noktada müdahale etmek istiyorsak onu seçiyoruz.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Filterlar Controllerlar içerisine gelen request'e müdahale etmek için kullanılır. Request gelmeden önce,geldikten sonra vs.
            //Filter kullanma amacımız Validation hatası var ise bizim kendi custom modelimizi dönmemize imkan tanır. Bu sayede daha action metota girmeden custom modelimi dönmüş olacağım. 
            //FluentValidation (ProductDtoValidator) daki hatalar direkt olarak ModelState'e yüklenir o sebeple ModelState.IsValid komutunu FluentValidation ile birlikte kullanabiliriz.
            //Burada modelstate olmadığı durumda "FluentValidation" tarafından gelen hazır responsa müdahale ederek bizim custom olarak hazırladığımız modeli gönderiyoruz.
            if (!context.ModelState.IsValid)
            {
                //ModelState.Values üzerinden hataları aldık. SelectMany bize gelen Dictionaryden tek bir property almamızı sağladı.
                var errors=context.ModelState.Values.SelectMany(x => x.Errors).Select(x=> x.ErrorMessage).ToList();
                //Client hatası olduğu için "BadRequestObjectResult" üzerinden "CustomResponseDto" olarak responselar için hazırladığımız modelimizi dönüyoruz.
                context.Result=new BadRequestObjectResult(CustomResponseDto<NoContentDto>.Fail(400,errors));
            }
        }


    }
}
