using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IVehiculoRepositorio : IRepositorio<Vehiculo>
    {
        Task<List<VehiculoDTO>> ObtenerPorPrestador(int prestadorId);
        Task<Vehiculo> RegistrarVehiculo(RegistroVehiculoDTO dto);
    }
}