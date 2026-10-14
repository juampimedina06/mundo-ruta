using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios;

public class MensajeRepositorio : Repositorio<Mensaje>, IMensajeRepositorio
{
    private readonly AppDbContext context;

    public MensajeRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Guardar un nuevo mensaje
    public async Task<Mensaje> CrearMensaje(Mensaje mensaje)
    {
        mensaje.EnviadoEn = DateTime.UtcNow;
        context.Mensajes.Add(mensaje);
        await context.SaveChangesAsync();
        return mensaje;
    }

    // Obtener los mensajes de un chat específico (ordenados del más antiguo al más reciente)
    // limite y offset para paginación. 50 es la cantidad máxima de mensajes que querés traer por llamada
    // Si no le pasás ningún valor por defecto trae 50 y el offset por defecto es 0 Indica cuántos mensajes querés saltearte
    public async Task<List<Mensaje>> ObtenerPorChatId(int idChat, int limite = 50, int offset = 0)
    {
        return await context.Mensajes
            .Include(m => m.Emisor)
            .Include(m => m.Destinatario)
            .Where(m => m.IdChat == idChat)
            .OrderBy(m => m.EnviadoEn)
            .Skip(offset)
            .Take(limite)
            .ToListAsync();
    }

    // Obtener un mensaje por su ID
    public async Task<Mensaje?> ObtenerPorId(int idMensaje)
    {
        return await context.Mensajes
            .Include(m => m.Emisor)
            .FirstOrDefaultAsync(m => m.Id == idMensaje);
    }

    // Eliminar un mensaje por ID
    public async Task<bool> EliminarAsync(int idMensaje)
    {
        var mensaje = await context.Mensajes.FindAsync(idMensaje);
        if (mensaje == null) return false;

        context.Mensajes.Remove(mensaje);
        await context.SaveChangesAsync();
        return true;
    }
}