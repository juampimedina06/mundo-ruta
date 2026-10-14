using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface ICargaRepositorio
    {
        Task<bool> AsignarViaje(int cargaId, int viajeId);
        Task<List<Carga>> ObtenerPorViaje(int viajeId);
    }
}