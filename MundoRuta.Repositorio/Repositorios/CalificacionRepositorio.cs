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
}