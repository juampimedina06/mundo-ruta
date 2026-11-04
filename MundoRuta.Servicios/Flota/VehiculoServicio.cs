using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Flota
{
    public class VehiculoServicio : IVehiculoServicio
    {
        private const string RutaVehiculos = "api/prestador/{0}/vehiculos";
        private const string RutaAltaVehiculo = "api/prestador/vehiculo";
        private const string RutaTarifaKm = "api/prestador/{0}/tarifa-km";

        private readonly IHttpServices _http;

        public VehiculoServicio(IHttpServices http)
        {
            _http = http;
        }

        public Task<HttpResp<List<VehiculoDTO>>> ListarAsync(int prestadorId)
        {
            string url = string.Format(RutaVehiculos, prestadorId);
            return _http.GetAsync<List<VehiculoDTO>>(url);
        }

        public Task<HttpResp<VehiculoDTO>> RegistrarAsync(RegistroVehiculoDTO dto)
        {
            return _http.PostAsync<RegistroVehiculoDTO, VehiculoDTO>(RutaAltaVehiculo, dto);
        }

        public Task<HttpResp<TarifaKmDTO>> ObtenerTarifaAsync(int prestadorId)
        {
            string url = string.Format(RutaTarifaKm, prestadorId);
            return _http.GetAsync<TarifaKmDTO>(url);
        }

        public Task<HttpResp<TarifaKmDTO>> GuardarTarifaAsync(int prestadorId, decimal tarifaBaseKm)
        {
            string url = string.Format(RutaTarifaKm, prestadorId);
            TarifaKmDTO dto = new TarifaKmDTO { TarifaBaseKm = tarifaBaseKm };
            return _http.PutAsync<TarifaKmDTO, TarifaKmDTO>(url, dto);
        }
    }
}
