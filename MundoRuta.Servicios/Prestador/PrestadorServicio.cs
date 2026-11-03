using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Servicios.Prestador
{
    public sealed class PrestadorServicio : IPrestadorServicio
    {
        private readonly IHttpServices _httpServices;

        public PrestadorServicio(IHttpServices httpServices)
        {
            _httpServices = httpServices;
        }

        public Task<HttpResp<List<ViajePrestadorDTO>>> ObtenerViajesAsync(int prestadorId, string? estado = null)
        {
            string url = $"api/prestador/{prestadorId}/viajes";

            if (!string.IsNullOrWhiteSpace(estado))
            {
                url += $"?estado={Uri.EscapeDataString(estado)}";
            }

            return _httpServices.GetAsync<List<ViajePrestadorDTO>>(url);
        }

        public Task<HttpResp<ViajeDetalladoDTO>> ObtenerDetalleViajeAsync(int prestadorId, int viajeId)
        {
            return _httpServices.GetAsync<ViajeDetalladoDTO>($"api/prestador/{prestadorId}/viajes/{viajeId}");
        }
    }
}
