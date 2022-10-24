using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NLayer.Repository;
using NLayer.Service.Mapping;
using NLayer.Service.Validations;
using NLayer.Web.Filters;
using NLayer.Web.Services;
using NLayer.WEB.Modules;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());

//AutoMapper'� tan�ml�yoruz. Assembly olarak typeof kabul etti�i i�in MapProfile olarak yazd�k. E�er ki birden fazla MapProfile tan�mlam�� olsayd�k Assembly mant���yla klas�r� g�stermemiz gerekirdi.
builder.Services.AddAutoMapper(typeof(MapProfile));

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

//Mvc projemizi Apiye dahil etmek i�in olu�turdu�umuz ProductApiService ve CategoryApiService i burada tan�mlad�k.
builder.Services.AddHttpClient<ProductApiService>(options =>
{
    //appsetting de tan�mlad���m�z BaseUrl'i verdik.
    options.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
});

builder.Services.AddHttpClient<CategoryApiService>(options =>
{
    //appsetting de tan�mlad���m�z BaseUrl'i verdik.
    options.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
});

builder.Services.AddScoped(typeof(NotFoundFilter<>));

//Autofac k�t�phanesini y�kledikten sonra kullanmak i�in yaz�yoruz.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

//Buradan Autofac kullanarak yazd���m�z RepoServiceModule'� dahil ediyoruz.
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));

//Cache'i burada tan�ml�yoruz
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
