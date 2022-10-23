using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.Web.Filters
{
    public class NotFoundFilter<T>:IAsyncActionFilter where T : BaseEntity
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
            var anyEntity = await _service.AnyAsync(x => x.Id == id);
            //Yani data var ise yine yoluna devam edecek
            if (anyEntity)
            {
                await next.Invoke();
                return;
            }
            //Mvc olduğu için ErrorViewModel ile birlikte Error sayfasına gönderdik.
            var errorViewModel = new ErrorViewModel();
            errorViewModel.Errors.Add($"{typeof(T).Name}({id}) not found");
            //Error sayfasına giderken "errorViewModel" datasını da beraberinde götürecek.
            context.Result=new RedirectToActionResult("Error","Home",errorViewModel);
        }
    }
}
