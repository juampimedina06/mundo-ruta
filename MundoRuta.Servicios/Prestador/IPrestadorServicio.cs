using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Prestador
{
    public interface IPrestadorServicio
    {
        Task<HttpResp<List<ViajePrestadorDTO>>> ObtenerViajesAsync(int prestadorId, string? estado = null);

        Task<HttpResp<ViajeDetalladoDTO>> ObtenerDetalleViajeAsync(int prestadorId, int viajeId);
      
        Task<HttpResp<List<ViajePrestadorDTO>>> ObtenerSolicitudesAsync(int prestadorId, string? estado = null);

        Task<HttpResp<List<ChoferDTO>>> ObtenerChoferesAsync(int prestadorId);

        Task<HttpResp<List<VehiculoDTO>>> ObtenerVehiculosAsync(int prestadorId);

        Task<HttpResp<object>> ResponderAsync(int viajeId, int prestadorId, ResponderSolicitudDTO dto);

        Task<HttpResp<object>> ContraOfertarAsync(int viajeId, int prestadorId, ContraOfertaDTO dto);
    }
}
