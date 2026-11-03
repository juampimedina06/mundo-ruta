using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Admin
{
    public class AdminServicio : IAdminServicio
    {
        private const string RutaDashboard = "api/admin/dashboard";
        private const string RutaSolicitudesPrestadores = "api/admin/prestadores/solicitudes";

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

        // Consulta las empresas de transporte registradas mediante GET api/admin/prestadores/solicitudes.
        public async Task<HttpResp<List<PrestadorSolicitudDTO>>> ObtenerSolicitudesPrestadoresAsync()
        {
            return await _http.GetAsync<List<PrestadorSolicitudDTO>>(RutaSolicitudesPrestadores);
        }

        // Cambia el estado de una empresa mediante PUT api/admin/prestadores/{id}/estado.
        public async Task<HttpResp<object>> ActualizarEstadoPrestadorAsync(int id, string estado)
        {
            ActualizarEstadoPrestadorDTO dto = new ActualizarEstadoPrestadorDTO { Estado = estado };
            string url = "api/admin/prestadores/" + id + "/estado";

            return await _http.PutAsync<ActualizarEstadoPrestadorDTO>(url, dto);
        }
    }
}
