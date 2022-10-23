using Microsoft.AspNetCore.Diagnostics;
using NLayer.Core.DTOs;
using NLayer.Service.Exceptions;
using System.Text.Json;

namespace NLayer.API.Middlewares
{
    //"UseExceptionHandler" hazır bir middleware'dir. Bu middleware uygulamada herhangi bir hata fırlatıldığında hatayı yakalar. Bu middleware'i customize ederek kendi "CustomResponseDto" nesnemizi response olarak dönebiliriz. Mvc tarafında ise yine bu middleware'i customize ederek herhangi bir sayfaya yönlendirebilirim.
    public static class UseCustomExceptionHandler
    {
        //Buradan extension metot ile program.cs app..... olarak yazdığımız tarafa ulaşıyoruz IApplication Builder sayesinde
        public static void UseCustomException(this IApplicationBuilder app)
        {
            //app.UseExceptionHandler()=> hazır sunulan exception'dır. Bu middleware uygulamanın herhangi bir yerinde hata alındığında kendi modeli ile bu hatayı response olarak verir. Biz burada bu modelin yerine bizim "CustomResponseDto" olarak oluşturduğumuz modeli göndermek için bu özelleştirmeyi yapıyoruz.
            app.UseExceptionHandler(config =>
            {
                //Run komutuyla middlewarelerde ilerlerken eğerki bir exception var ise buradan ileri gitmeyecek geri dönecek.(Sonlandırıcı middleware)-(Kısa devre)
                config.Run(async context =>
                {
                    //Context app içinde ne oluyor,kim kiminle iletişime geçiyor,arka planda çalışan işlemler nasıl dönüyor hepsinin bir arada tutulduğu bir arayüz diyebiliriz. Activity Context ve App Context en sık kullanılan contextlerdir.
                    context.Response.ContentType = "application/json";
                    //Fırlatılan hatayı bu exceptionFeature içine Get<IExceptionHandlerFeature>() ile alıyoruz.
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    //Client side exception adında bir class oluşturarak atılan hataların client mı ya da server tabanlı mı olduğunu ayırt etmek için yazıyoruz.
                    var statusCode = exceptionFeature.Error switch
                    {
                        //Hatanın tipi ClientSideException ise geriye 400 dön demek. 
                        ClientSideException => 400,
                        NotFoundException => 404,
                        //_ ile bunların dışında birşey ise 500 ata demek. Zaten Client tabanlı değil ise server tabanlı bir hatadır.
                        _ => 500
                    };
                    context.Response.StatusCode = statusCode;
                    //Response'u oluşturuyoruz.
                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode, exceptionFeature.Error.Message);
                    //Kendimiz bir middleware yazdığımız için json'a kendimiz çevirmemiz gerekiyor. Bu hazır middlewarelerde otomatik olarak yapılıyor.
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                });
            });
        }
    }
}
