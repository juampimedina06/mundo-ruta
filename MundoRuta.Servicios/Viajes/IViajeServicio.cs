using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Viajes
{
    // Contrato de las operaciones de viaje del pasajero.
    public interface IViajeServicio
    {
        Task<HttpResp<SolicitudViajeCreadaDTO>> SolicitarAsync(SolicitarViajeDTO request);
    }
}
