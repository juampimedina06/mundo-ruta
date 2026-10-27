using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Auth
{
    public interface IAuthServicio
    {
        Task<HttpResp<LoginResponseDto>> LoginAsync(LoginRequestDto request);

        Task<HttpResp<int>> RegistrarAsync(RegisterRequestDto request);

        void Logout();
    }
}
