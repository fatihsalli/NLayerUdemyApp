using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLayer.API.Filters;
using NLayer.API.Middlewares;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWorks;
using NLayer.Service.Mapping;
using NLayer.Service.Services;
using NLayer.Service.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//FluentValidation'� kullanmak i�in AddControllers'dan sonra "AddFluentValidation" methodu ile ekledik. "RegisterValidatorsFromAssemblyContaining" ile Validationlar� yapt���m�z class'� vererek bulmas�n� sa�lad�k. "ValidateFilterAttribute" ad�nda bir class olu�turduk bunu t�m Controllerlara tek tek tan�mlamak yerine options i�erisinde ekledik.
builder.Services.AddControllers(options=> options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x=> x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

//FluentValidationla d�nen response u pasif hale getirdik. Bu bask�lamay� yapmaya MVC taraf�nda gerek yoktur. Filter�n MVC taraf�nda aktif olma durumu yoktur.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Instancelar�m�z� al�yoruz.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//IGenericRepository birden fazla tip alsayd� o zaman <,> => 2 tane <,,> => 3 tane olarak 
//builder.Services.AddScoped(typeof(IGenericRepository<A,B,C>), typeof(GenericRepository<A,B,C>);
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
//AutoMapper'� tan�ml�yoruz. Assembly olarak typeof kabul etti�i i�in MapProfile olarak yazd�k. E�er ki birden fazla MapProfile tan�mlam�� olsayd�k Assembly mant���yla klas�r� g�stermemiz gerekirdi.
builder.Services.AddAutoMapper(typeof(MapProfile));
//Product �zelinde olu�turdu�umuz repository ve servicelerin instance �n� al�yoruz.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
//Category �zelinde olu�turdu�umuz repository ve servicelerin instance �n� al�yoruz.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


//Database ba�lant�
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"),option=>
    {
        //option.MigrationsAssembly("NLayer/Repository");
        //�stteki gibi yazmak yerine dinamik �ekilde vermek i�in Assembly kulland�k. B�ylece Repository ismi de�i�se de bulabilmesi i�in.
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);

    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Burada app. ile tan�mlanan t�m methodlar birer Middleware'dir. Middleware'lerden ge�erek Controllerdaki actiona gelir.
//Middleware'i aktif hale getirmek i�in burada tetikledik. Hata oldu�u i�in a�a��daki middlewarelerden yukar�da olmas� �nemlidir.
app.UserCustomException();

app.UseAuthorization();

app.MapControllers();

app.Run();
