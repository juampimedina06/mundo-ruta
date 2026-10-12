using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IChoferRepositorio :IRepositorio<Chofer>
    {
        Task CambiarEstado(int choferId, string nuevoEstado);
        Task<Chofer> CrearChoferConUsuario(ChoferAltaDTO dto);
        Task<List<ChoferDTO>> ObtenerPorPrestador(int prestadorId);
    }
}