using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Viajes
{
    public class ViajePasajeroServicio : IViajePasajeroServicio
    {
        private readonly IHttpServices _http;

        public ViajePasajeroServicio(IHttpServices http)
        {
            _http = http;
        }

        public Task<HttpResp<List<OfertaViajeDTO>>> ObtenerOfertasAsync(int viajeId)
        {
            return _http.GetAsync<List<OfertaViajeDTO>>($"api/pasajero/viajes/{viajeId}/ofertas");
        }

        public Task<HttpResp<object>> SeleccionarOfertaAsync(int viajeId, int ofertaId)
        {
            return _http.PutAsync($"api/pasajero/viajes/{viajeId}/seleccionar-oferta/{ofertaId}");
        }

        public Task<HttpResp<ViajeDetalladoDTO>> ObtenerViajeAsync(int usuarioId, int viajeId)
        {
            return _http.GetAsync<ViajeDetalladoDTO>($"api/pasajero/{usuarioId}/viajes/{viajeId}");
        }

        public Task<HttpResp<object>> CancelarViajeAsync(int viajeId, CancelarViajeDTO dto)
        {
            return _http.PutAsync<CancelarViajeDTO>($"api/pasajero/viajes/{viajeId}/cancelar", dto);
        }

        public Task<HttpResp<List<MensajeDTO>>> ObtenerMensajesAsync(int viajeId)
        {
            return _http.GetAsync<List<MensajeDTO>>($"api/chat/viajes/{viajeId}/mensajes");
        }

        public Task<HttpResp<object>> EnviarMensajeAsync(int viajeId, CrearMensajeDTO dto)
        {
            return _http.PostAsync<CrearMensajeDTO>($"api/chat/viajes/{viajeId}/mensajes", dto);
        }
    }
}
