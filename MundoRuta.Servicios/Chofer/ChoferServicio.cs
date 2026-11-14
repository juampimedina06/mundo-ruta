using System.Collections.Generic;
using System.Threading.Tasks;
using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Servicios.Chofer
{
    public sealed class ChoferServicio : IChoferServicio
    {
        private const string RutaBase = "api/chofer";
        private const string RutaChat = "api/chat";

        private readonly IHttpServices _http;

        public ChoferServicio(IHttpServices http)
        {
            _http = http;
        }

        public Task<HttpResp<ViajeChoferDTO>> ObtenerViajeActivoAsync(int choferId)
        {
            return _http.GetAsync<ViajeChoferDTO>($"{RutaBase}/{choferId}/viaje-activo");
        }

        public Task<HttpResp<object>> AvanzarEstadoViajeAsync(int viajeId)
        {
            return _http.PutAsync<object>($"{RutaBase}/viajes/{viajeId}/avanzar-estado", new { });
        }

        public Task<HttpResp<object>> ValidarEntregaAsync(int viajeId, ValidarEntregaDTO dto)
        {
            return _http.PostAsync<ValidarEntregaDTO, object>($"{RutaBase}/viajes/{viajeId}/validar-entrega", dto);
        }

        public Task<HttpResp<List<ChoferViajeHistorialDTO>>> ObtenerHistorialAsync(int choferId)
        {
            return _http.GetAsync<List<ChoferViajeHistorialDTO>>($"{RutaBase}/{choferId}/historial");
        }

        public Task<HttpResp<ChoferPerfilDTO>> ObtenerPerfilAsync(int choferId)
        {
            return _http.GetAsync<ChoferPerfilDTO>($"{RutaBase}/{choferId}/perfil");
        }

        public Task<HttpResp<object>> ActualizarPerfilAsync(int choferId, ActualizarPerfilDTO dto)
        {
            return _http.PutAsync<ActualizarPerfilDTO, object>($"{RutaBase}/{choferId}/perfil", dto);
        }

        public Task<HttpResp<List<MensajeDTO>>> ObtenerMensajesChatAsync(int viajeId)
        {
            return _http.GetAsync<List<MensajeDTO>>($"{RutaChat}/viajes/{viajeId}/mensajes");
        }

        public Task<HttpResp<object>> EnviarMensajeChatAsync(int viajeId, CrearMensajeDTO dto)
        {
            return _http.PostAsync<CrearMensajeDTO, object>($"{RutaChat}/viajes/{viajeId}/mensajes", dto);
        }
    }
}
