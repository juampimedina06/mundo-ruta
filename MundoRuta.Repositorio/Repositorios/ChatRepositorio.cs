using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios;

public class ChatRepositorio : Repositorio<Chat>, IChatRepositorio
{
    private readonly AppDbContext context;

    public ChatRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Crear un nuevo chat
    public async Task<Chat> CrearChat(Chat chat)
    {
        if (chat.FechaInicio == default)
        {
            chat.FechaInicio = DateTime.UtcNow;
        }

        context.Chats.Add(chat);
        await context.SaveChangesAsync();
        return chat;
    }

    // Obtener un chat por su ID con el viaje asociado
    public async Task<Chat?> ObtenerPorId(int idChat)
    {
        return await context.Chats
            .Include(c => c.IdViaje)
            .FirstOrDefaultAsync(c => c.Id == idChat);
    }

    // Obtener el chat vinculado a un viaje específico
    public async Task<Chat?> ObtenerPorViajeId(int idViaje)
    {
        return await context.Chats
            .Include(c => c.IdViaje)
            .FirstOrDefaultAsync(c => c.IdViaje != null && c.IdViaje.Id == idViaje);
    }

    // Obtener todos los chats en los que participa un usuario (donde haya sido emisor o destinatario)
    public async Task<List<Chat>> ObtenerPorUsuarioId(int idUsuario)
    {
        return await context.Mensajes
            .Where(m => m.IdEmisor == idUsuario || m.IdDestinatario == idUsuario)
            .Select(m => m.Chat)
            .Distinct()
            .Include(c => c.IdViaje)
            .ToListAsync();
    }

    // Eliminar o cerrar un chat por ID
    public async Task<bool> EliminarChat(int idChat)
    {
        var chat = await context.Chats.FindAsync(idChat);
        if (chat == null) return false;

        context.Chats.Remove(chat);
        await context.SaveChangesAsync();
        return true;
    }
}