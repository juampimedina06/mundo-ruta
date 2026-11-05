using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Usuarios
{
    public interface IUsuarioServicio
    {
        Task<HttpResp<UsuarioDTO>> ObtenerPorIdAsync(int id);

        Task<HttpResp<object>> ActualizarAsync(int id, UpdateUserRequestDto request);
    }
}
