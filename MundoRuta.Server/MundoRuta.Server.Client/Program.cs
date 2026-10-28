using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MundoRuta.Server.Client.Servicios;
using MundoRuta.Servicios.Extensions;
using MundoRuta.Servicios.ServiciosHTTP;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMundoRutaServicios(new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped<SesionPersistencia>();

builder.Services.AddScoped<IHttpServices, HttpServices>();


await builder.Build().RunAsync();
