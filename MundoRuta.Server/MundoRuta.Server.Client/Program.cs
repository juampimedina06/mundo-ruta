using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MundoRuta.Server.Client.Infraestructura;
using MundoRuta.Servicios.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMundoRutaServicios(new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped<SesionPersistencia>();

await builder.Build().RunAsync();
