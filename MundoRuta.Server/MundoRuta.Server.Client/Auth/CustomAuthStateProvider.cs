using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MundoRuta.Servicios.Sesion;
using System.Security.Claims;

namespace MundoRuta.Server.Client.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly SesionEstado _sesionEstado;

        public CustomAuthStateProvider(IJSRuntime jsRuntime, SesionEstado sesionEstado)
        {
            _jsRuntime = jsRuntime;
            _sesionEstado = sesionEstado;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var idStr = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", ""mr_id"");
                var nombre = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", ""mr_nombre"");
                var rol = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", ""mr_rol"");

                if (int.TryParse(idStr, out int id) && id > 0 && !string.IsNullOrWhiteSpace(rol))
                {
                    _sesionEstado.Iniciar(id, nombre ?? string.Empty, rol);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                        new Claim(ClaimTypes.Name, nombre ?? string.Empty),
                        new Claim(ClaimTypes.Role, rol)
                    };

                    var identity = new ClaimsIdentity(claims, ""mundo-ruta"");
                    var user = new ClaimsPrincipal(identity);
                    return new AuthenticationState(user);
                }
            }
            catch { }

            _sesionEstado.Cerrar();
            var anonymous = new ClaimsIdentity();
            return new AuthenticationState(new ClaimsPrincipal(anonymous));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
