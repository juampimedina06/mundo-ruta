using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Viajes
{
    public class ViajeServicio : IViajeServicio
    {
        private const string RutaSolicitar = "api/pasajero/viajes/solicitudes";

        private readonly IHttpServices _http;

        public ViajeServicio(IHttpServices http)
        {
            _http = http;
        }

        public Task<HttpResp<SolicitudViajeCreadaDTO>> SolicitarAsync(SolicitarViajeDTO request)
        {
            ArgumentNullException.ThrowIfNull(request);

            // El backend abre la subasta con Monto: el precio ofrecido se envía en ambos campos.
            if (request.PrecioOfrecido.HasValue)
            {
                request.Monto = request.PrecioOfrecido.Value;
            }
            else
            {
                request.PrecioOfrecido = request.Monto;
            }

            return _http.PostAsync<SolicitarViajeDTO, SolicitudViajeCreadaDTO>(RutaSolicitar, request);
        }
    }
}
