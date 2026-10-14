using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IMensajeRepositorio
    {
        Task<Mensaje> CrearMensaje(Mensaje mensaje);
        Task<bool> EliminarAsync(int idMensaje);
        Task<List<Mensaje>> ObtenerPorChatId(int idChat, int limite = 50, int offset = 0);
        Task<Mensaje?> ObtenerPorId(int idMensaje);
    }
}