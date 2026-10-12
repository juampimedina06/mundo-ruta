using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IChatRepositorio
    {
        Task<Chat> CrearChat(Chat chat);
        Task<bool> EliminarChat(int idChat);
        Task<Chat?> ObtenerPorId(int idChat);
        Task<List<Chat>> ObtenerPorUsuarioId(int idUsuario);
        Task<Chat?> ObtenerPorViajeId(int idViaje);
    }
}