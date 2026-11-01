using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Admin
{
    public interface IAdminServicio
    {
        Task<HttpResp<AdminDashboardDTO>> ObtenerDashboardAsync();
    }
}
