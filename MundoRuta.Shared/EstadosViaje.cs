using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared
{
    // Contiene los estados permitidos para un viaje.
    public static class EstadosViaje
    {
        public const string Buscando = "BUSCANDO"; // El viaje está esperando que un prestador lo acepte.
        public const string Asignado = "ASIGNADO"; // El viaje ya tiene un prestador asignado.
        public const string CargaSubida = "CARGA_SUBIDA"; // La carga ya fue subida.
        public const string EnTransito = "EN_TRANSITO"; // El viaje está en tránsito.
        public const string LlegadaDestino = "LLEGADA_DESTINO"; // El viaje llegó a su destino.
        public const string Finalizado = "FINALIZADO"; // El viaje terminó correctamente.
        public const string Cancelado = "CANCELADO"; // El viaje fue cancelado.
        public const string Contraofertado = "CONTRAOFERTADO"; // Hay una contraoferta pendiente.
        public const string Rechazado = "RECHAZADO"; // El viaje fue rechazado.
    }
}
