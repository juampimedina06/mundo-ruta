using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MundoRuta.Server.Client.Servicios;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<ViajeHubService>();

await builder.Build().RunAsync();
