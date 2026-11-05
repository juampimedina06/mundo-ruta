using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Flota
{
    // Operaciones de la flota del prestador: listado, alta y tarifa base por km.
    public interface IVehiculoServicio
    {
        Task<HttpResp<List<VehiculoDTO>>> ListarAsync(int prestadorId);

        Task<HttpResp<VehiculoDTO>> RegistrarAsync(RegistroVehiculoDTO dto);

        Task<HttpResp<TarifaKmDTO>> ObtenerTarifaAsync(int prestadorId);

        Task<HttpResp<TarifaKmDTO>> GuardarTarifaAsync(int prestadorId, decimal tarifaBaseKm);
    }
}
