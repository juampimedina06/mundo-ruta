namespace MundoRuta.Shared.DTO
{
    // Resumen consolidado de las métricas del panel de administración.
    public class AdminDashboardDTO
    {
        /// <summary>
        /// Cantidad total de usuarios del sistema, sumando pasajeros, prestadores y choferes (no incluye administradores).
        /// </summary>
        public int TotalUsuarios { get; set; }

        /// <summary>
        /// Cantidad de usuarios con rol "Usuario", considerados pasajeros.
        /// </summary>
        public int TotalPasajeros { get; set; }

        /// <summary>
        /// Cantidad de usuarios con rol "Prestador", sin importar su estado.
        /// </summary>
        public int TotalPrestadores { get; set; }

        /// <summary>
        /// Cantidad de usuarios con rol "Chofer".
        /// </summary>
        public int TotalChoferes { get; set; }

        /// <summary>
        /// Cantidad de prestadores cuyo estado es "PENDIENTE" de aprobación.
        /// </summary>
        public int PrestadoresPendientes { get; set; }

        /// <summary>
        /// Cantidad total de viajes registrados en el sistema.
        /// </summary>
        public int TotalViajes { get; set; }

        /// <summary>
        /// Cantidad de viajes actualmente activos (en curso de cualquier estado operativo).
        /// </summary>
        public int ViajesActivos { get; set; }

        /// <summary>
        /// Monto total transaccionado por los viajes finalizados.
        /// </summary>
        public decimal VolumenTransaccionado { get; set; }

        /// <summary>
        /// Total de comisiones acumuladas en las liquidaciones registradas.
        /// </summary>
        public decimal Comisiones { get; set; }

        /// <summary>
        /// Fecha y hora (UTC) en la que se generó el resumen.
        /// </summary>
        public DateTime FechaCorte { get; set; }
    }
}
