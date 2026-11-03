using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Servicios.Prestador
{
    public interface IPrestadorServicio
    {
        Task<HttpResp<List<ViajePrestadorDTO>>> ObtenerViajesAsync(int prestadorId, string? estado = null);

        Task<HttpResp<ViajeDetalladoDTO>> ObtenerDetalleViajeAsync(int prestadorId, int viajeId);
    }
}
