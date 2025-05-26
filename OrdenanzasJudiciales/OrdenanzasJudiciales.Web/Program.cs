//using Microsoft.EntityFrameworkCore;
//using OrdenanzasJudiciales.Aplicacion.Interfaces;
//using OrdenanzasJudiciales.Infraestructura.Data.Context;
//using OrdenanzasJudiciales.Infraestructura.Data.Repositorios;


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

//builder.Services.AddScoped<IProcesosOrdenanzasRepositorio, ProcesosOrdenanzasRepositorio>();
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));


using Microsoft.EntityFrameworkCore;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Infraestructura.Data.Context;
using OrdenanzasJudiciales.Infraestructura.Data.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllersWithViews();

// Registrar la cadena de conexión (SQLExpress)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar el repositorio
builder.Services.AddScoped<IProcesosOrdenanzasRepositorio, ProcesosOrdenanzasRepositorio>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

