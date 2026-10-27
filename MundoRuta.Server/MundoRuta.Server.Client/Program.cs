using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MundoRuta.Servicios.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMundoRutaServicios(new Uri(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();
