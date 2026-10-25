using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared
{
    // Contiene los estados permitidos para un viaje.
    public static class EstadosViaje
    {

        public const string Buscando = "buscando"; // El viaje está esperando que un prestador lo acepte.

        public const string Asignado = "asignado"; // El viaje ya tiene un prestador asignado.

        public const string CargaSubida = "carga-subida"; // La carga ya fue subida.

        public const string EnTransito = "en-transito"; // El viaje está en tránsito.

        public const string LlegadaDestino = "llegada-destino"; // El viaje llegó a su destino.

        public const string Finalizado = "finalizado"; // El viaje terminó correctamente.

        public const string Cancelado = "cancelado"; // El viaje fue cancelado.

    }
}
