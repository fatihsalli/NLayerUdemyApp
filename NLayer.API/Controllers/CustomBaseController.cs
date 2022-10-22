using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;

namespace NLayer.API.Controllers
{
    //CustomBaseController'ı category ve product controller'a miras verdiğimiz için tekrar route vs. yazmamıza gerek yok. Buradan alacaktır.
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        //CustomBaseController oluşturduk bunu oluşturma amacımız Api controller tarafında return Ok,Bad Request vs. yazmaktansa oluşturduğumuz CustomResponseDto'yu kullanarak response'a göre status code veriyoruz.
        //Endpoint olmadığını belirtmek için "NonAction" tanımlaması yaptık. Aksi halde Swagger bu Action için endpoint oluşturur.Get'i veya post'u olmadığı içinde hata fırlatır.
        [NonAction]
        public IActionResult CreateActionResult<T>(CustomResponseDto<T> response)
        {
            if (response.StatusCode == 204)
                //ObjectResult IActionResulttan miras aldığı için Ok,BadRequest yerine biz bu şekilde kendimiz belirleyebiliyoruz.
                return new ObjectResult(null)
                {
                    StatusCode = response.StatusCode
                };

            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

    }
}
