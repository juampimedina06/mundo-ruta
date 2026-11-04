using MundoRuta.Shared;

namespace MundoRuta.Server.Client.Infraestructura
{
    // Centraliza las etiquetas y la normalización de los estados de viaje en la UI.
    public static class EstadosViajeUI
    {
        public const string Todos = "TODOS";

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

        public static string Etiqueta(string? estado)
        {
            switch (Clave(estado))
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

        public static bool Coincide(string? estado, string filtro)
        {
            if (string.Equals(filtro, Todos, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return string.Equals(Clave(estado), Clave(filtro), StringComparison.OrdinalIgnoreCase);
        }

        private static string Clave(string? estado)
        {
            return EstadosViaje.Normalizar(estado);
        }
    }

    public sealed class OpcionEstado
    {
        public string Valor { get; init; } = string.Empty;

        public string Etiqueta { get; init; } = string.Empty;
    }
}
