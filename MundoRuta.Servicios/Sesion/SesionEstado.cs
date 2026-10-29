namespace MundoRuta.Servicios.Sesion
{
    public class SesionEstado
    {
        public int Id { get; private set; }
        public string? Nombre { get; private set; }
        public string? Rol { get; private set; }
        public bool EstaLogueado => Id > 0;
        public bool Restaurada { get; private set; }

        public event Action? CambioSesion;

        public void Iniciar(int id, string nombre, string rol)
        {
            Id = id;
            Nombre = nombre;
            Rol = rol;
            CambioSesion?.Invoke();
        }

        public void Cerrar()
        {
            Id = 0;
            Nombre = null;
            Rol = null;
            CambioSesion?.Invoke();
        }

        public bool TieneRol(string rol)
        {
            return !string.IsNullOrWhiteSpace(Rol)
                && string.Equals(Rol, rol, StringComparison.OrdinalIgnoreCase);
        }

        // Marca la sesión como restaurada una sola vez y notifica el cambio.
        public void MarcarRestaurada()
        {
            if (Restaurada)
            {
                return;
            }

            Restaurada = true;
            CambioSesion?.Invoke();
        }
    }
}
