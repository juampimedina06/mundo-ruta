using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios;

public class LiquidacionRepositorio : Repositorio<Liquidacion>, ILiquidacionRepositorio
{
    private readonly AppDbContext context;

    public LiquidacionRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // 1. Guardar una nueva liquidación
    public async Task<Liquidacion> CrearLiquidacion(Liquidacion liquidacion)
    {
        if (liquidacion.Fecha == default)
        {
            liquidacion.Fecha = DateTime.UtcNow;
        }

        context.Liquidaciones.Add(liquidacion);
        await context.SaveChangesAsync();
        return liquidacion;
    }

    // 2. Obtener una liquidación por su ID (incluyendo los datos del usuario)
    public async Task<Liquidacion?> ObtenerPorId(int idLiquidacion)
    {
        return await context.Liquidaciones
            .Include(l => l.Usuario)
            .FirstOrDefaultAsync(l => l.Id == idLiquidacion);
    }

    // 3. Obtener el historial de liquidaciones de un usuario (prestador)
    public async Task<List<Liquidacion>> ObtenerPorUsuarioId(int idUsuario)
    {
        return await context.Liquidaciones
            .Where(l => l.IdUsuario == idUsuario)
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }

    // 4. Obtener las liquidaciones pendientes de pago (Estado = false)
    public async Task<List<Liquidacion>> ObtenerPendientes()
    {
        return await context.Liquidaciones
            .Include(l => l.Usuario)
            .Where(l => !l.Estado)
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }

    // 5. Obtener todas las liquidaciones registradas
    public async Task<List<Liquidacion>> ObtenerTodas()
    {
        return await context.Liquidaciones
            .Include(l => l.Usuario)
            .OrderByDescending(l => l.Fecha)
            .ToListAsync();
    }
}