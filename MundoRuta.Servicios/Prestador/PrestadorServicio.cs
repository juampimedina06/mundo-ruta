using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Servicios.Prestador
{
    public sealed class PrestadorServicio : IPrestadorServicio
    {
        private const string RutaBase = "api/prestador";

        private readonly IHttpServices _http;

        public PrestadorServicio(IHttpServices http)
        {
            _http = http;
        }


        public Task<HttpResp<List<ViajePrestadorDTO>>> ObtenerViajesAsync(int prestadorId, string? estado = null)
        {
            string url = $"{RutaBase}/{prestadorId}/viajes";

            if (!string.IsNullOrWhiteSpace(estado))
            {
                url += "?estado=" + Uri.EscapeDataString(estado);
            }

            return _http.GetAsync<List<ViajePrestadorDTO>>(url);
        }

        public Task<HttpResp<ViajeDetalladoDTO>> ObtenerDetalleViajeAsync(int prestadorId, int viajeId)
        {
            return _http.GetAsync<ViajeDetalladoDTO>($"{RutaBase}/{prestadorId}/viajes/{viajeId}");
        }


        public Task<HttpResp<List<ViajePrestadorDTO>>> ObtenerSolicitudesAsync(int prestadorId, string? estado = null)
        {
            string url = $"{RutaBase}/{prestadorId}/solicitudes";

            if (!string.IsNullOrWhiteSpace(estado))
            {
                url += "?estado=" + Uri.EscapeDataString(estado);
            }

            return _http.GetAsync<List<ViajePrestadorDTO>>(url);
        }

        public Task<HttpResp<List<ChoferDTO>>> ObtenerChoferesAsync(int prestadorId)
        {
            return _http.GetAsync<List<ChoferDTO>>($"{RutaBase}/{prestadorId}/choferes");
        }

        public Task<HttpResp<List<VehiculoDTO>>> ObtenerVehiculosAsync(int prestadorId)
        {
            return _http.GetAsync<List<VehiculoDTO>>($"{RutaBase}/{prestadorId}/vehiculos");
        }

        public Task<HttpResp<object>> ResponderAsync(int viajeId, int prestadorId, ResponderSolicitudDTO dto)
        {
            string url = $"{RutaBase}/viajes/{viajeId}/responder?idPrestador={prestadorId}";
            return _http.PutAsync<ResponderSolicitudDTO>(url, dto);
        }

        public Task<HttpResp<object>> ContraOfertarAsync(int viajeId, int prestadorId, ContraOfertaDTO dto)
        {
            string url = $"{RutaBase}/viajes/{viajeId}/contraofertar?idPrestador={prestadorId}";
            return _http.PutAsync<ContraOfertaDTO>(url, dto);
        }
    }
}