using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios;

public interface ICalificacionRepositorio : IRepositorio<Calificacion>
{
    Task<Calificacion> CrearCalificacion(Calificacion calificacion);
    Task<List<Calificacion>> ObtenerPorPrestador(int idPrestador);
}