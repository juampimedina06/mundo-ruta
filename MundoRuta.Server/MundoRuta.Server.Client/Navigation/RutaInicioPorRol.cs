using MundoRuta.Shared;

namespace MundoRuta.Server.Client.Navigation
{
    // Resuelve la ruta de inicio que corresponde a cada rol.
    public static class RutaInicioPorRol
    {
        public static string Obtener(string? rol)
        {
            if (string.IsNullOrWhiteSpace(rol))
            {
                return "/";
            }

            if (string.Equals(rol, Roles.Usuario, StringComparison.OrdinalIgnoreCase))
            {
                return "/usuario";
            }

            if (string.Equals(rol, Roles.Prestador, StringComparison.OrdinalIgnoreCase))
            {
                return "/prestador";
            }

            if (string.Equals(rol, Roles.Chofer, StringComparison.OrdinalIgnoreCase))
            {
                return "/chofer";
            }

            if (string.Equals(rol, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return "/admin";
            }

            return "/";
        }
    }
}
