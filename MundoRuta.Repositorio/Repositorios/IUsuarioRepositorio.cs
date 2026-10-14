using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        Task<Usuario?> ObtenerPorId(int id);
        Task<Usuario?> ObtenerPorEmail(string email);
        Task<bool> EmailExiste(string email, int idExcluir = 0);
        Task<Usuario?> ValidarCredenciales(string email, string password);
        Task<List<PrestadorListadoDTO>> ObtenerPrestadoresPorEstado(string estado);
        Task<bool> CambiarEstado(int id, string nuevoEstado);
        Task<int> ContarPorRolYEstado(string rol, string? estado = null);
    }
}