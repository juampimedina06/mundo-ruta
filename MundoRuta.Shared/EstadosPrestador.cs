namespace MundoRuta.Shared
{
    // Contiene los estados permitidos para una empresa de transporte (rol Prestador).
    public static class EstadosPrestador
    {
        public const string Pendiente = "PENDIENTE"; // La empresa espera que el administrador evalúe su alta.
        public const string Aprobado = "APROBADO"; // La empresa puede iniciar sesión y operar.
        public const string Rechazado = "RECHAZADO"; // El administrador rechazó el alta.
        public const string Suspendido = "SUSPENDIDO"; // La empresa fue suspendida y no puede operar.

        private static readonly string[] EstadosAsignables = { Aprobado, Rechazado, Suspendido };

        // Indica si el administrador puede mover una solicitud a ese estado.
        public static bool EsAsignable(string? estado)
        {
            return estado is not null && Array.IndexOf(EstadosAsignables, estado) >= 0;
        }
    }
}
