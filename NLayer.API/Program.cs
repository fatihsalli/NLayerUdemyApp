using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWorks;
using NLayer.Service.Mapping;
using NLayer.Service.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
