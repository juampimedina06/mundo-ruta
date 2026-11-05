using MundoRuta.Shared;

namespace MundoRuta.Server.Client.Infraestructura
{
    // Etapas del seguimiento del viaje vistas por el pasajero.
    public enum EtapaViaje
    {
        Buscando = 0,
        Asignado = 1,
        CargaSubida = 2,
        EnTransito = 3,
        LlegadaDestino = 4
    }

    // Centraliza las etiquetas y la normalización de los estados de viaje en la UI.
    // La BD guarda los estados en dos formatos ("CARGA_SUBIDA" en EstadosViaje y "Carga-subida" en ChoferController),
    // por eso toda lectura pasa por el normalizador antes de compararse.
    public static class EstadosViajeUI
    {
        public const string Todos = "TODOS";

        // Opciones para filtros (vista del prestador).
        public static IReadOnlyList<OpcionEstado> Opciones { get; } = new List<OpcionEstado>
        {
            new() { Valor = Todos, Etiqueta = "Todos" },
            new() { Valor = EstadosViaje.Buscando, Etiqueta = "Buscando" },
            new() { Valor = EstadosViaje.Asignado, Etiqueta = "Asignado" },
            new() { Valor = EstadosViaje.CargaSubida, Etiqueta = "Carga subida" },
            new() { Valor = EstadosViaje.EnTransito, Etiqueta = "En tránsito" },
            new() { Valor = EstadosViaje.LlegadaDestino, Etiqueta = "Llegada a destino" },
            new() { Valor = EstadosViaje.Finalizado, Etiqueta = "Finalizado" },
            new() { Valor = EstadosViaje.Contraofertado, Etiqueta = "Contraofertado" },
            new() { Valor = EstadosViaje.Cancelado, Etiqueta = "Cancelado" },
            new() { Valor = EstadosViaje.Rechazado, Etiqueta = "Rechazado" }
        };

        private static readonly Dictionary<EtapaViaje, string> Etiquetas = new()
        {
            { EtapaViaje.Buscando, "Buscando conductor" },
            { EtapaViaje.Asignado, "Asignado" },
            { EtapaViaje.CargaSubida, "Carga subida" },
            { EtapaViaje.EnTransito, "En tránsito" },
            { EtapaViaje.LlegadaDestino, "Llegada a destino" }
        };

        // Secuencia exacta que debe mostrar el seguimiento.
        public static readonly IReadOnlyList<EtapaViaje> Secuencia = new[]
        {
            EtapaViaje.Buscando,
            EtapaViaje.Asignado,
            EtapaViaje.CargaSubida,
            EtapaViaje.EnTransito,
            EtapaViaje.LlegadaDestino
        };

        public static string Normalizar(string? estado)
        {
            return (estado ?? string.Empty).Trim().Replace('_', '-').ToLowerInvariant();
        }

        // Etiqueta genérica por estado (listados y filtros).
        public static string Etiqueta(string? estado)
        {
            switch (Normalizar(estado))
            {
                case "buscando":
                    return "Buscando";
                case "asignado":
                    return "Asignado";
                case "carga-subida":
                    return "Carga subida";
                case "en-transito":
                    return "En tránsito";
                case "llegada-destino":
                    return "Llegada a destino";
                case "finalizado":
                    return "Finalizado";
                case "contraofertado":
                    return "Contraofertado";
                case "cancelado":
                    return "Cancelado";
                case "rechazado":
                    return "Rechazado";
                default:
                    return string.IsNullOrWhiteSpace(estado) ? "Sin estado" : estado.Trim();
            }
        }

        // Etiqueta de una etapa del seguimiento (vista del pasajero).
        public static string Etiqueta(EtapaViaje etapa)
        {
            return Etiquetas[etapa];
        }

        // Etiqueta para el seguimiento del pasajero: "contraofertado" se muestra como "Buscando conductor".
        public static string EtiquetaSeguimiento(string? estado)
        {
            EtapaViaje? etapa = ObtenerEtapa(estado);

            if (etapa.HasValue)
            {
                return Etiquetas[etapa.Value];
            }

            return string.IsNullOrWhiteSpace(estado) ? "Sin estado" : estado.Trim();
        }

        public static bool Coincide(string? estado, string filtro)
        {
            if (string.Equals(filtro, Todos, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return string.Equals(Normalizar(estado), Normalizar(filtro), StringComparison.OrdinalIgnoreCase);
        }

        // Devuelve la etapa del seguimiento o null si el estado no pertenece a la secuencia.
        public static EtapaViaje? ObtenerEtapa(string? estado)
        {
            switch (Normalizar(estado))
            {
                case "buscando":
                case "contraofertado": // El viaje sigue en subasta: se muestra como "buscando".
                    return EtapaViaje.Buscando;
                case "asignado":
                    return EtapaViaje.Asignado;
                case "carga-subida":
                    return EtapaViaje.CargaSubida;
                case "en-transito":
                    return EtapaViaje.EnTransito;
                case "llegada-destino":
                    return EtapaViaje.LlegadaDestino;
                default:
                    return null;
            }
        }

        // Estados en los que el viaje está activo y el chat con el chofer queda habilitado.
        public static bool ChatHabilitado(string? estado)
        {
            string normalizado = Normalizar(estado);
            return normalizado == "asignado"
                || normalizado == "carga-subida"
                || normalizado == "en-transito"
                || normalizado == "llegada-destino";
        }

        public static bool EstaBuscando(string? estado)
        {
            string normalizado = Normalizar(estado);
            return normalizado == "buscando" || normalizado == "contraofertado";
        }

        public static bool EstaCancelado(string? estado)
        {
            string normalizado = Normalizar(estado);
            return normalizado == "cancelado" || normalizado == "rechazado";
        }

        public static bool Termino(string? estado)
        {
            string normalizado = Normalizar(estado);
            return normalizado == "finalizado" || normalizado == "llegada-destino";
        }
    }

    public sealed class OpcionEstado
    {
        public string Valor { get; init; } = string.Empty;

        public string Etiqueta { get; init; } = string.Empty;
    }
}