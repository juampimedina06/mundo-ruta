using MundoRuta.Shared.DTO;
using MundoRuta.Servicios.ServiciosHTTP;

namespace MundoRuta.Servicios.Usuarios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IHttpServices _http;

        public UsuarioServicio(IHttpServices http)
        {
            _http = http;
        }

        public Task<HttpResp<UsuarioDTO>> ObtenerPorIdAsync(int id)
        {
            return _http.GetAsync<UsuarioDTO>($"api/usuario/{id}");
        }

        public Task<HttpResp<object>> ActualizarAsync(int id, UpdateUserRequestDto request)
        {
            return _http.PutAsync<UpdateUserRequestDto>($"api/usuario/{id}", request);
        }
    }
}
