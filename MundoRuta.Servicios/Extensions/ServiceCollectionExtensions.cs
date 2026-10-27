using Microsoft.Extensions.DependencyInjection;
using MundoRuta.Servicios.Realtime;

namespace MundoRuta.Servicios.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMundoRutaServicios(this IServiceCollection services, Uri baseAddress)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(baseAddress);

            services.AddScoped<HttpClient>(_ =>
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = baseAddress;
                return httpClient;
            });

            services.AddScoped<IViajeHubService, ViajeHubService>();

            return services;
        }
    }
}
