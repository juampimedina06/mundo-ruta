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

    // La BD guarda los estados en dos formatos ("CARGA_SUBIDA" en EstadosViaje y "Carga-subida" en ChoferController),
    // por eso toda lectura pasa por el normalizador antes de compararse.
    public static class EstadosViajeUI
    {
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

        public static string Etiqueta(EtapaViaje etapa)
        {
            return Etiquetas[etapa];
        }

        public static string Etiqueta(string? estado)
        {
            EtapaViaje? etapa = ObtenerEtapa(estado);

            if (etapa.HasValue)
            {
                return Etiquetas[etapa.Value];
            }

            return string.IsNullOrWhiteSpace(estado) ? "Sin estado" : estado.Trim();
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
}
