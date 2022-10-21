using Microsoft.AspNetCore.Diagnostics;
using NLayer.Core.DTOs;
using NLayer.Service.Exceptions;
using System.Text.Json;

namespace NLayer.API.Middlewares
{
    //Use Exception Handler zaten halihazırda Api ile bize sunulur. Burada ise extension bir method oluşturarak exception'ı özelleştirmiş oluyoruz.
    public static class UseCustomExceptionHandler
    {
        //Buradan program.cs app..... olarak yazdığımız tarafa ulaşıyoruz IApplication Builder sayesinde
        public static void UserCustomException(this IApplicationBuilder app)
        {
            //app.UseExceptionHandler()=> hazır sunulan exception'dır.
            app.UseExceptionHandler(config=>
            {
                //Run komutuyla middlewarelerde ilerlerken eğerki bir exception var ise buradan ileri gitmeyecek geri dönecek.(Sonlandırıcı middleware)
                config.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    //Fırlatılan hatayı bu exceptionFeature içine Get<IExceptionHandlerFeature>() ile alıyoruz.
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    //Client side exception adında bir class oluşturarak atılan hataların client mı ya da server tabanlı mı olduğunu ayırt etmek için yazıyoruz.
                    var statusCode=exceptionFeature.Error switch
                    {
                        ClientSideException=>400,
                        _ =>500
                    };
                    context.Response.StatusCode = statusCode;

                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode, exceptionFeature.Error.Message);
                    //Kendimiz bir middleware yazdığımız için json'a kendimiz çevirmemiz gerekiyor. Bu middlewarelerde otomatik olarak yapılıyor.
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                });
            });
        }
    }
}
