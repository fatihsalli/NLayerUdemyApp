using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLayer.API.Filters;
using NLayer.API.Middlewares;
using NLayer.API.Modules;
using NLayer.Repository;
using NLayer.Service.Mapping;
using NLayer.Service.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//FluentValidation'ý kullanmak için AddControllers'dan sonra "AddFluentValidation" methodu ile ekledik. "RegisterValidatorsFromAssemblyContaining" ile Validationlarý yaptýðýmýz class'ý vererek bulmasýný saðladýk. "ValidateFilterAttribute" adýnda bir class oluþturduk bunu tüm Controllerlara tek tek tanýmlamak yerine options içerisinde ekledik. Bu sayede global olarak tüm controllerlara bu filterý uygulamýþ olduk.
builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

//FluentValidationla dönen response'u aþaðýda pasif hale getirdik. Bu baskýlamayý yapmaya MVC tarafýnda gerek yoktur. Filterýn MVC tarafýnda aktif olma durumu yoktur. Ancak Api tarafýnda biz custom olarak oluþturduðumuz response'u dönmek için aþaðýdaki ayarý yapmalýyýz. Bununla birlikte filter yazmamýz gereklidir. Üst satýrda yazýldý.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //Kendi döndüðü model filtresini inaktif duruma getirdik.
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cache'i burada tanýmlýyoruz
builder.Services.AddMemoryCache();
//Generic olduðu için typeof ile belirttik. Service'i implemente ettiðimiz için burada belirtmemiz gerekiyor.
builder.Services.AddScoped(typeof(NotFoundFilter<>));
//AutoMapper'ý tanýmlýyoruz. Assembly olarak typeof kabul ettiði için MapProfile olarak yazdýk. Eðer ki birden fazla MapProfile tanýmlamýþ olsaydýk Assembly mantýðýyla klasörü göstermemiz gerekirdi.
builder.Services.AddAutoMapper(typeof(MapProfile));

//Instancelarýmýzý alýyoruz. AutoFac kütüphanesi ile repository,service ve unitofworklerin Scope olarak instance'ýný aldýk o sebeple burada gerek kalmadý buradaki instancelara.


//Database baðlantý
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        //option.MigrationsAssembly("NLayer/Repository");
        //Üstteki gibi yazmak yerine dinamik þekilde vermek için Assembly kullandýk. Böylece Repository ismi deðiþse de bulabilmesi için.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);

    });
});

//Autofac kütüphanesini yükledikten sonra kullanmak için yazýyoruz.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

//Buradan Autofac kullanarak yazdýðýmýz RepoServiceModule'ü dahil ediyoruz.
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

//Middlewareleri tanýmladýðýmýz alan
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Burada app. ile tanýmlanan tüm methodlar birer Middleware'dir. Middleware'lerden geçerek Controllerdaki actiona gelir.
//Middleware'i aktif hale getirmek için burada tetikledik. Hata olduðu için aþaðýdaki middlewarelerden yukarýda olmasý önemlidir.
//Middleware'lerin çalýþma mantýðý þudur; request ile beraber tetiklenerek ilerler ve response'u client'a gönderebilmek için tekrar geriye uðrayarak döngü tamamlanýr. Yani [] bu bir middleware olsun birden fazla middleware þu þekilde gösterilebilir [[[ ]]] ve döngü tamamlanýr.
app.UseCustomException();

app.UseAuthorization();

app.MapControllers();

app.Run();
