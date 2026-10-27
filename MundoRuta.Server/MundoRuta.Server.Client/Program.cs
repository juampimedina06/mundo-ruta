using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MundoRuta.Server.Client.Auth;
using MundoRuta.Server.Client.Auth.Services;
using MundoRuta.Servicios.Extensions;
using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Servicios.Sesion;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMundoRutaServicios(new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped<IHttpServices, HttpServices>();
builder.Services.AddScoped<SesionEstado>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<IAuthClientService, AuthClientService>();

await builder.Build().RunAsync();
