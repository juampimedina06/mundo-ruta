using Microsoft.JSInterop;
using MundoRuta.Servicios.Auth;
using MundoRuta.Servicios.Sesion;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Client.Auth.Services
{
    public class AuthClientService : IAuthClientService
    {
        private readonly IAuthServicio _authServicio;
        private readonly SesionEstado _sesionEstado;
        private readonly CustomAuthStateProvider _authStateProvider;
        private readonly IJSRuntime _jsRuntime;

        public AuthClientService(IAuthServicio authServicio, SesionEstado sesionEstado, CustomAuthStateProvider authStateProvider, IJSRuntime jsRuntime)
        {
            _authServicio = authServicio;
            _sesionEstado = sesionEstado;
            _authStateProvider = authStateProvider;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var request = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _authServicio.LoginAsync(request);

            if (!response.Error && response.Respuesta is not null)
            {
                await GuardarSesionAsync(response.Respuesta);
                _authStateProvider.NotifyAuthenticationStateChanged();
                return true;
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            _authServicio.Logout();
            await LimpiarSesionAsync();
            _authStateProvider.NotifyAuthenticationStateChanged();
        }

        private async Task GuardarSesionAsync(LoginResponseDto dto)
        {
            await _jsRuntime.InvokeVoidAsync(""localStorage.setItem"", ""mr_id"", dto.Id.ToString());
            await _jsRuntime.InvokeVoidAsync(""localStorage.setItem"", ""mr_nombre"", dto.Nombre ?? string.Empty);
            await _jsRuntime.InvokeVoidAsync(""localStorage.setItem"", ""mr_rol"", dto.Rol ?? string.Empty);
        }

        private async Task LimpiarSesionAsync()
        {
            await _jsRuntime.InvokeVoidAsync(""localStorage.removeItem"", ""mr_id"");
            await _jsRuntime.InvokeVoidAsync(""localStorage.removeItem"", ""mr_nombre"");
            await _jsRuntime.InvokeVoidAsync(""localStorage.removeItem"", ""mr_rol"");
        }
    }
}
