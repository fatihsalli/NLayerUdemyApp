using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Filters
{
    //Exceptionlarımız global olarak yazıldı. Filter neden yazıyoruz? Herhangi bir entity için data=null olduğunda ek business yapılması gerekebilir. Örneğin mesaj kuyruğa gidip mesaj atsın gibi veya kullanıcıya email atmak gibi.
    public class NotFoundFilter<T> : IAsyncActionFilter where T : BaseEntity
    {
        private readonly IService<T> _service;

        public NotFoundFilter(IService<T> service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Gelen id'yi yakaladık.
            var idValue = context.ActionArguments.Values.FirstOrDefault();
            //Id null ise demekki GetById veya benzeri bir metot kullanılmamış demektir. Next.Invoke() ile yoluna devam edebilir ve return ile metottan çıkarak.
            if (idValue == null)
            {
                await next.Invoke();
                return;
            }
            //Cast işlemi yaptık
            var id = (int)idValue;
            //(x=> x.Id==id) => x.Id'ye ulaşabilmek için yukarıda where T:class yerine BaseEntity yaptık.
            var anyEntity =await _service.AnyAsync(x=> x.Id==id);
            //Yani data var ise yine yoluna devam edecek
            if (anyEntity)
            {
                await next.Invoke();
                return;
            }
            //Burada data yok olarak yani Not found olarak response döndük.
            context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({id}) not found"));
        }
    }
}
