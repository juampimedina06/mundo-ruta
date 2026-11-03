using System;
using System.Collections.Generic;
using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Admin
{
    public class AdminServicio : IAdminServicio
    {
        private const string RutaDashboard = "api/admin/dashboard";
        private const string RutaSolicitudesPrestadores = "api/admin/prestadores/solicitudes";
        private const string RutaUsuarios = "api/admin/usuarios";
        private const string RutaComisiones = "api/admin/comisiones";

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

        // Consulta el directorio de usuarios mediante GET api/admin/usuarios.
        public async Task<HttpResp<List<AdminUsuarioListadoDTO>>> ObtenerUsuariosAsync()
        {
            return await _http.GetAsync<List<AdminUsuarioListadoDTO>>(RutaUsuarios);
        }

        // Consulta la ficha completa de un usuario mediante GET api/admin/usuarios/{id}.
        public async Task<HttpResp<AdminUsuarioDetalleDTO>> ObtenerUsuarioAsync(int id)
        {
            string url = RutaUsuarios + "/" + id;

            return await _http.GetAsync<AdminUsuarioDetalleDTO>(url);
        }

        // Consulta el reporte financiero de comisiones mediante GET api/admin/comisiones.
        public async Task<HttpResp<AdminComisionesReporteDTO>> ObtenerComisionesAsync(AdminFiltroComisionesDTO? filtro = null)
        {
            List<string> parametros = new List<string>();

            if (filtro != null)
            {
                if (filtro.FechaDesde.HasValue)
                {
                    parametros.Add("fechaDesde=" + Uri.EscapeDataString(filtro.FechaDesde.Value.ToString("yyyy-MM-dd")));
                }

                if (filtro.FechaHasta.HasValue)
                {
                    parametros.Add("fechaHasta=" + Uri.EscapeDataString(filtro.FechaHasta.Value.ToString("yyyy-MM-dd")));
                }

                if (filtro.PrestadorId.HasValue && filtro.PrestadorId.Value > 0)
                {
                    parametros.Add("prestadorId=" + filtro.PrestadorId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filtro.EstadoViaje))
                {
                    parametros.Add("estadoViaje=" + Uri.EscapeDataString(filtro.EstadoViaje.Trim()));
                }
            }

            string url = RutaComisiones;
            if (parametros.Count > 0)
            {
                url += "?" + string.Join("&", parametros);
            }

            return await _http.GetAsync<AdminComisionesReporteDTO>(url);
        }
    }
}
