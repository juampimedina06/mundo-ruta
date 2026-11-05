using Microsoft.Extensions.DependencyInjection;
using MundoRuta.Servicios.Admin;
using MundoRuta.Servicios.Auth;
using MundoRuta.Servicios.Prestador;
using MundoRuta.Servicios.Flota;
using MundoRuta.Servicios.Realtime;
using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Servicios.Sesion;
using MundoRuta.Servicios.Viajes;
using MundoRuta.Servicios.Usuarios;

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
            services.AddScoped<IViajeServicio, ViajeServicio>();
            services.AddScoped<IUsuarioServicio, UsuarioServicio>();
            services.AddScoped<IPrestadorServicio, PrestadorServicio>();
            services.AddScoped<IVehiculoServicio, VehiculoServicio>();
            services.AddScoped<IAdminServicio, AdminServicio>();
            services.AddScoped<IViajeHubService, ViajeHubService>();
            services.AddScoped<IViajePasajeroServicio, ViajePasajeroServicio>();

            return services;
        }
    }
}
