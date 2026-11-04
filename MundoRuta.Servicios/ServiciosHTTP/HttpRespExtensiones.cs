using System.Text.Json;

namespace MundoRuta.Servicios.ServiciosHTTP
{
    // Utilidades para presentar los mensajes de las respuestas HTTP.
    public static class HttpRespExtensiones
    {
        public static async Task<string> ObtenerMensajeAsync<T>(this HttpResp<T> respuesta, string mensajePorDefecto)
        {
            ArgumentNullException.ThrowIfNull(respuesta);

            if (!respuesta.Error)
            {
                return respuesta.Mensaje ?? mensajePorDefecto;
            }

            if (respuesta.Response is not null)
            {
                string contenido = await respuesta.Response.Content.ReadAsStringAsync();
                string? mensaje = ExtraerMensaje(contenido, respuesta.Response.Content.Headers.ContentType?.MediaType);

                if (!string.IsNullOrWhiteSpace(mensaje))
                {
                    return mensaje.Trim().Trim('"');
                }
            }

            return respuesta.Mensaje ?? mensajePorDefecto;
        }

        // Evita volcar HTML u otras estructuras crudas en la interfaz.
        private static string? ExtraerMensaje(string contenido, string? tipoContenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return null;
            }

            string recortado = contenido.TrimStart();

            bool esHtml = (tipoContenido?.Contains("html", StringComparison.OrdinalIgnoreCase) ?? false)
                || recortado.StartsWith("<", StringComparison.Ordinal);

            if (esHtml)
            {
                return null;
            }

            bool esJson = (tipoContenido?.Contains("json", StringComparison.OrdinalIgnoreCase) ?? false)
                || recortado.StartsWith("{", StringComparison.Ordinal)
                || recortado.StartsWith("[", StringComparison.Ordinal);

            if (!esJson)
            {
                return recortado.Length <= 300 ? contenido : null;
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(contenido);

                if (documento.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (string nombre in new[] { "mensaje", "message", "error", "detail", "title" })
                    {
                        if (documento.RootElement.TryGetProperty(nombre, out JsonElement valor)
                            && valor.ValueKind == JsonValueKind.String)
                        {
                            return valor.GetString();
                        }
                    }
                }
            }
            catch (JsonException)
            {
            }

            return null;
        }
    }
}
