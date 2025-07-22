using Microsoft.EntityFrameworkCore;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Infraestructura.Data.Context;
using OrdenanzasJudiciales.Infraestructura.Data.Juzgados;
using OrdenanzasJudiciales.Infraestructura.Data.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProcesosOrdenanzasRepositorio, ProcesosOrdenanzasRepositorio>();
builder.Services.AddScoped<DatosOrdenanzasJuzgados>();
builder.Services.AddScoped<IServicioJuzgados, DatosOrdenanzasJuzgados>();

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Configurar rutas
app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=ProcesosOrdenanzas}/{action=Index}/{id?}");
app.Run();