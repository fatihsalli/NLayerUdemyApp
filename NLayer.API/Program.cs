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

//FluentValidation'ý kullanmak için AddControllers'dan sonra "AddFluentValidation" methodu ile ekledik. "RegisterValidatorsFromAssemblyContaining" ile Validationlarý yaptýðýmýz class'ý vererek bulmasýný saðladýk. "ValidateFilterAttribute" adýnda bir class oluþturduk bunu tüm Controllerlara tek tek tanýmlamak yerine options içerisinde ekledik.
builder.Services.AddControllers(options=> options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x=> x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

//FluentValidationla dönen response u pasif hale getirdik. Bu baskýlamayý yapmaya MVC tarafýnda gerek yoktur. Filterýn MVC tarafýnda aktif olma durumu yoktur.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Instancelarýmýzý alýyoruz.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//IGenericRepository birden fazla tip alsaydý o zaman <,> => 2 tane <,,> => 3 tane olarak 
//builder.Services.AddScoped(typeof(IGenericRepository<A,B,C>), typeof(GenericRepository<A,B,C>);
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
//AutoMapper'ý tanýmlýyoruz. Assembly olarak typeof kabul ettiði için MapProfile olarak yazdýk. Eðer ki birden fazla MapProfile tanýmlamýþ olsaydýk Assembly mantýðýyla klasörü göstermemiz gerekirdi.
builder.Services.AddAutoMapper(typeof(MapProfile));
//Product özelinde oluþturduðumuz repository ve servicelerin instance ýný alýyoruz.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
//Category özelinde oluþturduðumuz repository ve servicelerin instance ýný alýyoruz.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


//Database baðlantý
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"),option=>
    {
        //option.MigrationsAssembly("NLayer/Repository");
        //Üstteki gibi yazmak yerine dinamik þekilde vermek için Assembly kullandýk. Böylece Repository ismi deðiþse de bulabilmesi için.
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

//Burada app. ile tanýmlanan tüm methodlar birer Middleware'dir. Middleware'lerden geçerek Controllerdaki actiona gelir.
//Middleware'i aktif hale getirmek için burada tetikledik. Hata olduðu için aþaðýdaki middlewarelerden yukarýda olmasý önemlidir.
app.UserCustomException();

app.UseAuthorization();

app.MapControllers();

app.Run();
