using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios;

public interface ICalificacionRepositorio : IRepositorio<Calificacion>
{
    Task<Calificacion> CrearCalificacion(Calificacion calificacion);
    Task<List<Calificacion>> ObtenerPorPrestador(int idPrestador);
    Task<(double? Promedio, int Total)> ObtenerPromedioRecibido(int usuarioId);
    Task<(double? Promedio, int Total)> ObtenerPromedioOtorgado(int usuarioId);
}