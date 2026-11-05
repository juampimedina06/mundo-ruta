using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios;

public class CalificacionRepositorio : Repositorio<Calificacion>, ICalificacionRepositorio
{
    private readonly AppDbContext context;

    public CalificacionRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // 1. Método para guardar una nueva calificación
    public async Task<Calificacion> CrearCalificacion(Calificacion calificacion)
    {
        context.Calificaciones.Add(calificacion);
        await context.SaveChangesAsync();
        return calificacion;
    }

    // 2. Método para obtener todas las calificaciones de un prestador
    public async Task<List<Calificacion>> ObtenerPorPrestador(int idPrestador)
    {
        return await context.Calificaciones
            .Where(c => c.IdUsuarioPrestador == idPrestador)
            .ToListAsync();
    }

    // 3. Promedio y total de calificaciones que recibió el usuario como prestador
    public async Task<(double? Promedio, int Total)> ObtenerPromedioRecibido(int usuarioId)
    {
        int total = await context.Calificaciones.CountAsync(c => c.IdUsuarioPrestador == usuarioId);
        if (total == 0)
        {
            return (null, 0);
        }

        double promedio = await context.Calificaciones
            .Where(c => c.IdUsuarioPrestador == usuarioId)
            .AverageAsync(c => (double)c.Puntaje);

        return (promedio, total);
    }

    // 4. Promedio y total de calificaciones que otorgó el usuario
    public async Task<(double? Promedio, int Total)> ObtenerPromedioOtorgado(int usuarioId)
    {
        int total = await context.Calificaciones.CountAsync(c => c.IdUsuario == usuarioId);
        if (total == 0)
        {
            return (null, 0);
        }

        double promedio = await context.Calificaciones
            .Where(c => c.IdUsuario == usuarioId)
            .AverageAsync(c => (double)c.Puntaje);

        return (promedio, total);
    }
}