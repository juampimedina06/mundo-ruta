using System.Text.Json;
using Microsoft.JSInterop;
using MundoRuta.Servicios.Sesion;

namespace MundoRuta.Server.Client.Servicios
{
    // Guarda y restaura la sesión del usuario en el almacenamiento local del navegador.
    public sealed class SesionPersistencia : IAsyncDisposable
    {
        private const string ClaveAlmacenamiento = "mundoruta.sesion";
        private static readonly JsonSerializerOptions OpcionesJson = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly SesionEstado _sesion;
        private readonly IJSRuntime _js;
        private bool _restaurando;
        private bool _restaurada;

        public SesionPersistencia(SesionEstado sesion, IJSRuntime js)
        {
            _sesion = sesion;
            _js = js;
            _sesion.CambioSesion += AlCambiarSesion;
        }

        public bool Restaurada => _restaurada;

        public async Task RestaurarAsync()
        {
            if (_restaurada || _restaurando)
            {
                return;
            }

            _restaurando = true;

            try
            {
                string? valor = await _js.InvokeAsync<string?>("localStorage.getItem", ClaveAlmacenamiento);

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    DatosSesion? datos = JsonSerializer.Deserialize<DatosSesion>(valor, OpcionesJson);

                    if (datos is not null && datos.Id > 0)
                    {
                        _sesion.Iniciar(datos.Id, datos.Nombre ?? string.Empty, datos.Rol ?? string.Empty);
                    }
                }
            }
            catch (JSException)
            {
                await EliminarAlmacenadoAsync();
            }
            finally
            {
                _restaurando = false;
                _restaurada = true;
                _sesion.MarcarRestaurada();
            }
        }

        public async Task GuardarAsync()
        {
            try
            {
                if (!_sesion.EstaLogueado)
                {
                    await _js.InvokeVoidAsync("localStorage.removeItem", ClaveAlmacenamiento);
                    return;
                }

                DatosSesion datos = new DatosSesion
                {
                    Id = _sesion.Id,
                    Nombre = _sesion.Nombre,
                    Rol = _sesion.Rol
                };

                string valor = JsonSerializer.Serialize(datos, OpcionesJson);
                await _js.InvokeVoidAsync("localStorage.setItem", ClaveAlmacenamiento, valor);
            }
            catch (JSException)
            {
                // El almacenamiento del navegador no está disponible.
            }
        }

        public ValueTask DisposeAsync()
        {
            _sesion.CambioSesion -= AlCambiarSesion;
            return ValueTask.CompletedTask;
        }

        private void AlCambiarSesion()
        {
            if (!_restaurada || _restaurando)
            {
                return;
            }

            _ = GuardarAsync();
        }

        private async Task EliminarAlmacenadoAsync()
        {
            try
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", ClaveAlmacenamiento);
            }
            catch (JSException)
            {
                // El almacenamiento del navegador no está disponible.
            }
        }

        private sealed class DatosSesion
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
            public string? Rol { get; set; }
        }
    }
}
