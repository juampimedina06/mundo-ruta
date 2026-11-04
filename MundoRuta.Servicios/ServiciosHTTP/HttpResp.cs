namespace MundoRuta.Servicios.ServiciosHTTP
{
    public class HttpResp<T>
    {
        private string? _mensaje;

        public bool Error { get; set; }
        public T? Respuesta { get; set; }
        public HttpResponseMessage? Response { get; set; }

        public string? Mensaje
        {
            get => _mensaje ?? ObtenerError();
            set => _mensaje = value;
        }

        public HttpResp(T? dato, bool error, HttpResponseMessage? response, string? mensaje)
        {
            Respuesta = dato;
            Error = error;
            Response = response;
            Mensaje = mensaje;
        }

        public string ObtenerError()
        {
            //si mensaje viene distinto a empy o null, se retorna el mensaje
            if (_mensaje != string.Empty && _mensaje != null)
            {
                return _mensaje;
            }

            if (Response == null)
            {
                return "No se obtuvo respuesta";

            }
            else
            {
                string Mensaje = "";
                var statusCode = Response.StatusCode;

                #region Mensaje segun el código de estado HTTP
                switch (statusCode)
                {
                    case System.Net.HttpStatusCode.Continue:
                        Mensaje = "Continuar";
                        break;
                    case System.Net.HttpStatusCode.SwitchingProtocols:
                        Mensaje = "Cambiando protocolos";
                        break;
                    case System.Net.HttpStatusCode.Processing:
                        Mensaje = "Procesando";
                        break;
                    case System.Net.HttpStatusCode.EarlyHints:
                        Mensaje = "Sugerencias tempranas";
                        break;
                    case System.Net.HttpStatusCode.OK:
                        Mensaje = "Solicitud exitosa";
                        break;
                    case System.Net.HttpStatusCode.Created:
                        Mensaje = "Recurso creado correctamente";
                        break;
                    case System.Net.HttpStatusCode.Accepted:
                        Mensaje = "Solicitud aceptada";
                        break;
                    case System.Net.HttpStatusCode.NonAuthoritativeInformation:
                        Mensaje = "Información no autorizada";
                        break;
                    case System.Net.HttpStatusCode.NoContent:
                        Mensaje = "Sin contenido";
                        break;
                    case System.Net.HttpStatusCode.ResetContent:
                        Mensaje = "Contenido restablecido";
                        break;
                    case System.Net.HttpStatusCode.PartialContent:
                        Mensaje = "Contenido parcial";
                        break;
                    case System.Net.HttpStatusCode.MultiStatus:
                        Mensaje = "Multiestado";
                        break;
                    case System.Net.HttpStatusCode.AlreadyReported:
                        Mensaje = "Ya reportado";
                        break;
                    case System.Net.HttpStatusCode.IMUsed:
                        Mensaje = "IM usado";
                        break;
                    case System.Net.HttpStatusCode.Ambiguous:
                        Mensaje = "Respuesta ambigua";
                        break;
                    case System.Net.HttpStatusCode.Moved:
                        Mensaje = "Recurso movido";
                        break;
                    case System.Net.HttpStatusCode.Found:
                        Mensaje = "Recurso encontrado";
                        break;
                    case System.Net.HttpStatusCode.RedirectMethod:
                        Mensaje = "Redirección de método";
                        break;
                    case System.Net.HttpStatusCode.NotModified:
                        Mensaje = "No modificado";
                        break;
                    case System.Net.HttpStatusCode.UseProxy:
                        Mensaje = "Usar proxy";
                        break;
                    case System.Net.HttpStatusCode.Unused:
                        Mensaje = "No usado";
                        break;
                    case System.Net.HttpStatusCode.RedirectKeepVerb:
                        Mensaje = "Redirección manteniendo verbo";
                        break;
                    case System.Net.HttpStatusCode.PermanentRedirect:
                        Mensaje = "Redirección permanente";
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        Mensaje = "Solicitud incorrecta - intente nuevamente";
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        Mensaje = "No autorizado";
                        break;
                    case System.Net.HttpStatusCode.PaymentRequired:
                        Mensaje = "Pago requerido";
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        Mensaje = "Prohibido";
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        Mensaje = "No encontrado";
                        break;
                    case System.Net.HttpStatusCode.MethodNotAllowed:
                        Mensaje = "Método no permitido";
                        break;
                    case System.Net.HttpStatusCode.NotAcceptable:
                        Mensaje = "No aceptable";
                        break;
                    case System.Net.HttpStatusCode.ProxyAuthenticationRequired:
                        Mensaje = "Autenticación de proxy requerida";
                        break;
                    case System.Net.HttpStatusCode.RequestTimeout:
                        Mensaje = "Tiempo de espera agotado";
                        break;
                    case System.Net.HttpStatusCode.Conflict:
                        Mensaje = "Conflicto";
                        break;
                    case System.Net.HttpStatusCode.Gone:
                        Mensaje = "Recurso no disponible";
                        break;
                    case System.Net.HttpStatusCode.LengthRequired:
                        Mensaje = "Longitud requerida";
                        break;
                    case System.Net.HttpStatusCode.PreconditionFailed:
                        Mensaje = "Precondición fallida";
                        break;
                    case System.Net.HttpStatusCode.RequestEntityTooLarge:
                        Mensaje = "Entidad de solicitud demasiado grande";
                        break;
                    case System.Net.HttpStatusCode.RequestUriTooLong:
                        Mensaje = "URI de solicitud demasiado larga";
                        break;
                    case System.Net.HttpStatusCode.UnsupportedMediaType:
                        Mensaje = "Tipo de medio no soportado";
                        break;
                    case System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
                        Mensaje = "Rango solicitado no satisfactorio";
                        break;
                    case System.Net.HttpStatusCode.ExpectationFailed:
                        Mensaje = "Expectativa fallida";
                        break;
                    case System.Net.HttpStatusCode.MisdirectedRequest:
                        Mensaje = "Solicitud mal dirigida";
                        break;
                    case System.Net.HttpStatusCode.UnprocessableEntity:
                        Mensaje = "Entidad no procesable";
                        break;
                    case System.Net.HttpStatusCode.Locked:
                        Mensaje = "Recurso bloqueado";
                        break;
                    case System.Net.HttpStatusCode.FailedDependency:
                        Mensaje = "Dependencia fallida";
                        break;
                    case System.Net.HttpStatusCode.UpgradeRequired:
                        Mensaje = "Actualización requerida";
                        break;
                    case System.Net.HttpStatusCode.PreconditionRequired:
                        Mensaje = "Precondición requerida";
                        break;
                    case System.Net.HttpStatusCode.TooManyRequests:
                        Mensaje = "Demasiadas solicitudes";
                        break;
                    case System.Net.HttpStatusCode.RequestHeaderFieldsTooLarge:
                        Mensaje = "Campos de cabecera demasiado grandes";
                        break;
                    case System.Net.HttpStatusCode.UnavailableForLegalReasons:
                        Mensaje = "No disponible por razones legales";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        Mensaje = "Error interno del servidor";
                        break;
                    case System.Net.HttpStatusCode.NotImplemented:
                        Mensaje = "No implementado";
                        break;
                    case System.Net.HttpStatusCode.BadGateway:
                        Mensaje = "Puerta de enlace incorrecta";
                        break;
                    case System.Net.HttpStatusCode.ServiceUnavailable:
                        Mensaje = "Servicio no disponible";
                        break;
                    case System.Net.HttpStatusCode.GatewayTimeout:
                        Mensaje = "Tiempo de espera de la puerta de enlace agotado";
                        break;
                    case System.Net.HttpStatusCode.HttpVersionNotSupported:
                        Mensaje = "Versión HTTP no soportada";
                        break;
                    case System.Net.HttpStatusCode.VariantAlsoNegotiates:
                        Mensaje = "La variante también negocia";
                        break;
                    case System.Net.HttpStatusCode.InsufficientStorage:
                        Mensaje = "Almacenamiento insuficiente";
                        break;
                    case System.Net.HttpStatusCode.LoopDetected:
                        Mensaje = "Bucle detectado";
                        break;
                    case System.Net.HttpStatusCode.NotExtended:
                        Mensaje = "No extendido";
                        break;
                    case System.Net.HttpStatusCode.NetworkAuthenticationRequired:
                        Mensaje = "Autenticación de red requerida";
                        break;
                    default:
                        Mensaje = "Código de estado desconocido";
                        break;
                }
                return Mensaje;
                #endregion

            }

        }
    }
}
