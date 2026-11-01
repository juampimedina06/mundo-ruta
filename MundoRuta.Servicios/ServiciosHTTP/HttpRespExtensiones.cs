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

                if (!string.IsNullOrWhiteSpace(contenido))
                {
                    return contenido.Trim().Trim('"');
                }
            }

            return respuesta.Mensaje ?? mensajePorDefecto;
        }
    }
}
