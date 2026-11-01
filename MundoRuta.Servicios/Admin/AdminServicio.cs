using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Admin
{
    public class AdminServicio : IAdminServicio
    {
        private const string RutaDashboard = "api/admin/dashboard";

        private readonly IHttpServices _http;

        public AdminServicio(IHttpServices http)
        {
            _http = http;
        }

        // Consulta el resumen del panel mediante GET api/admin/dashboard.
        public async Task<HttpResp<AdminDashboardDTO>> ObtenerDashboardAsync()
        {
            return await _http.GetAsync<AdminDashboardDTO>(RutaDashboard);
        }
    }
}
