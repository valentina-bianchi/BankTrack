using BankTrackWeb.Repositories;
using BankTrackWeb.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IGenericRepository<Cliente>, ClienteRepository>();
builder.Services.AddScoped<IGenericRepository<CuentaBancaria>, CuentaBancariaRepository>();
builder.Services.AddScoped<IGenericRepository<Categoria>, CategoriaRepository>();
builder.Services.AddScoped<IGenericRepository<TipoTransaccion>, TipoTransaccionRepository>();
builder.Services.AddScoped<TransaccionRepository>(); // Agregar esto



var app = builder.Build();
var cultureInfo = new CultureInfo("es-ES");
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new[] { cultureInfo },
    SupportedUICultures = new[] { cultureInfo }
});


// Añadir este middleware para que el model binder de ASP.NET use la cultura especificada
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new[] { cultureInfo },
    SupportedUICultures = new[] { cultureInfo }
});

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
    pattern: "{controller=Inicio}/{action=Login}/{id?}");

app.Run();
