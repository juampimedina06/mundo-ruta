using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Admin
{
    public interface IAdminServicio
    {
        Task<HttpResp<AdminDashboardDTO>> ObtenerDashboardAsync();

        Task<HttpResp<List<PrestadorSolicitudDTO>>> ObtenerSolicitudesPrestadoresAsync();

        Task<HttpResp<object>> ActualizarEstadoPrestadorAsync(int id, string estado);

        Task<HttpResp<List<AdminUsuarioListadoDTO>>> ObtenerUsuariosAsync();

        Task<HttpResp<AdminUsuarioDetalleDTO>> ObtenerUsuarioAsync(int id);

        Task<HttpResp<AdminComisionesReporteDTO>> ObtenerComisionesAsync(AdminFiltroComisionesDTO? filtro = null);
    }
}
