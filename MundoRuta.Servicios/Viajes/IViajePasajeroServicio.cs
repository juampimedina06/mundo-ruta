using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Viajes
{
    // Operaciones del pasajero sobre sus viajes: ofertas, selección, cancelación y chat.
    public interface IViajePasajeroServicio
    {
        Task<HttpResp<List<OfertaViajeDTO>>> ObtenerOfertasAsync(int viajeId);

        Task<HttpResp<object>> SeleccionarOfertaAsync(int viajeId, int ofertaId);

        Task<HttpResp<ViajeDetalladoDTO>> ObtenerViajeAsync(int usuarioId, int viajeId);

        Task<HttpResp<object>> CancelarViajeAsync(int viajeId, CancelarViajeDTO dto);

        Task<HttpResp<List<MensajeDTO>>> ObtenerMensajesAsync(int viajeId);

        Task<HttpResp<object>> EnviarMensajeAsync(int viajeId, CrearMensajeDTO dto);
    }
}
