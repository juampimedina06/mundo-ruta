using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.Server.Client.Pages;
using MundoRuta.Server.Components;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("ConnSqlServer")
    ?? throw new InvalidOperationException("No existe la conexión con la base de datos.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnSqlServer")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

#region Servicios
builder.Services.AddSwaggerGen(); //esto es para documentar la API
builder.Services.AddControllersWithViews();// Agregar servicios para controladores y vistas
builder.Services.AddRazorPages();// Agregar servicios para páginas Razor

builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
}); // Agregar un servicio HttpClient con la dirección base de la aplicación web
#endregion



#region Inyeccion de dependencias
//agregar repositorios e interfaz
#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#region Middlewares
app.MapControllers();// Mapear los controladores a las rutas correspondientes
#endregion


app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();// Habilitar el enrutamiento de la aplicación

app.UseAntiforgery();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
//todo esto es el mapeo de los componentes de Razor y Blazor, para que se puedan renderizar en el navegador

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MundoRuta.Server.Client._Imports).Assembly);

app.Run();
