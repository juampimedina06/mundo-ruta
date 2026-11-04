using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;
using MundoRuta.Servicios.Sesion;

namespace MundoRuta.Servicios.Auth
{
    public class AuthServicio : IAuthServicio
    {
        private const string RutaLogin = "api/auth/login";
        private const string RutaRegistro = "api/auth/register";

        private readonly IHttpServices _http;
        private readonly SesionEstado _sesion;

        public AuthServicio(IHttpServices http, SesionEstado sesion)
        {
            _http = http;
            _sesion = sesion;
        }

        public async Task<HttpResp<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            HttpResp<LoginResponseDto> respuesta = await _http.PostAsync<LoginRequestDto, LoginResponseDto>(RutaLogin, request);

            if (!respuesta.Error && respuesta.Respuesta is not null)
            {
                _sesion.Iniciar(respuesta.Respuesta.Id, respuesta.Respuesta.Nombre, respuesta.Respuesta.Rol);
            }

            return respuesta;
        }

        public Task<HttpResp<int>> RegistrarAsync(RegisterRequestDto request)
        {
            return _http.PostAsync<RegisterRequestDto, int>(RutaRegistro, request);
        }

        public void Logout()
        {
            _sesion.Cerrar();
        }
    }
}
