using Microsoft.Extensions.DependencyInjection;
using MundoRuta.Servicios.Admin;
using MundoRuta.Servicios.Auth;
using MundoRuta.Servicios.Flota;
using MundoRuta.Servicios.Realtime;
using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Servicios.Sesion;

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

            services.AddScoped<IHttpServices, HttpServices>();
            services.AddScoped<SesionEstado>();
            services.AddScoped<IAuthServicio, AuthServicio>();
            services.AddScoped<IVehiculoServicio, VehiculoServicio>();
            services.AddScoped<IAdminServicio, AdminServicio>();
            services.AddScoped<IViajeHubService, ViajeHubService>();

            return services;
        }
    }
}
