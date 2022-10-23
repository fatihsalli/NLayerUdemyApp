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

//FluentValidation'� kullanmak i�in AddControllers'dan sonra "AddFluentValidation" methodu ile ekledik. "RegisterValidatorsFromAssemblyContaining" ile Validationlar� yapt���m�z class'� vererek bulmas�n� sa�lad�k. "ValidateFilterAttribute" ad�nda bir class olu�turduk bunu t�m Controllerlara tek tek tan�mlamak yerine options i�erisinde ekledik. Bu sayede global olarak t�m controllerlara bu filter� uygulam�� olduk.
builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

//FluentValidationla d�nen response'u a�a��da pasif hale getirdik. Bu bask�lamay� yapmaya MVC taraf�nda gerek yoktur. Filter�n MVC taraf�nda aktif olma durumu yoktur. Ancak Api taraf�nda biz custom olarak olu�turdu�umuz response'u d�nmek i�in a�a��daki ayar� yapmal�y�z. Bununla birlikte filter yazmam�z gereklidir. �st sat�rda yaz�ld�.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //Kendi d�nd��� model filtresini inaktif duruma getirdik.
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cache'i burada tan�ml�yoruz
builder.Services.AddMemoryCache();
//Generic oldu�u i�in typeof ile belirttik. Service'i implemente etti�imiz i�in burada belirtmemiz gerekiyor.
builder.Services.AddScoped(typeof(NotFoundFilter<>));
//AutoMapper'� tan�ml�yoruz. Assembly olarak typeof kabul etti�i i�in MapProfile olarak yazd�k. E�er ki birden fazla MapProfile tan�mlam�� olsayd�k Assembly mant���yla klas�r� g�stermemiz gerekirdi.
builder.Services.AddAutoMapper(typeof(MapProfile));

//Instancelar�m�z� al�yoruz. AutoFac k�t�phanesi ile repository,service ve unitofworklerin Scope olarak instance'�n� ald�k o sebeple burada gerek kalmad� buradaki instancelara.


//Database ba�lant�
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        //option.MigrationsAssembly("NLayer/Repository");
        //�stteki gibi yazmak yerine dinamik �ekilde vermek i�in Assembly kulland�k. B�ylece Repository ismi de�i�se de bulabilmesi i�in.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);

    });
});

//Autofac k�t�phanesini y�kledikten sonra kullanmak i�in yaz�yoruz.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

//Buradan Autofac kullanarak yazd���m�z RepoServiceModule'� dahil ediyoruz.
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

//Middlewareleri tan�mlad���m�z alan
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Burada app. ile tan�mlanan t�m methodlar birer Middleware'dir. Middleware'lerden ge�erek Controllerdaki actiona gelir.
//Middleware'i aktif hale getirmek i�in burada tetikledik. Hata oldu�u i�in a�a��daki middlewarelerden yukar�da olmas� �nemlidir.
//Middleware'lerin �al��ma mant��� �udur; request ile beraber tetiklenerek ilerler ve response'u client'a g�nderebilmek i�in tekrar geriye u�rayarak d�ng� tamamlan�r. Yani [] bu bir middleware olsun birden fazla middleware �u �ekilde g�sterilebilir [[[ ]]] ve d�ng� tamamlan�r.
app.UseCustomException();

app.UseAuthorization();

app.MapControllers();

app.Run();
