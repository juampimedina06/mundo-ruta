using System.Collections.Generic;
using System.Threading.Tasks;
using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Servicios.Chofer
{
    public interface IChoferServicio
    {
        Task<HttpResp<ViajeChoferDTO>> ObtenerViajeActivoAsync(int choferId);

        Task<HttpResp<object>> AvanzarEstadoViajeAsync(int viajeId);

        Task<HttpResp<object>> ValidarEntregaAsync(int viajeId, ValidarEntregaDTO dto);

        Task<HttpResp<List<ChoferViajeHistorialDTO>>> ObtenerHistorialAsync(int choferId);

        Task<HttpResp<ChoferPerfilDTO>> ObtenerPerfilAsync(int choferId);

        Task<HttpResp<object>> ActualizarPerfilAsync(int choferId, ActualizarPerfilDTO dto);

        Task<HttpResp<List<MensajeDTO>>> ObtenerMensajesChatAsync(int viajeId);

        Task<HttpResp<object>> EnviarMensajeChatAsync(int viajeId, CrearMensajeDTO dto);
    }
}
