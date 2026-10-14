using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IServicioRepositorio
    {
        Task<Servicio> ObtenerOCrearPorDefecto();
    }
}