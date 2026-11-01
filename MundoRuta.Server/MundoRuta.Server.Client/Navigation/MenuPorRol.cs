using System.Collections.Generic;
using MundoRuta.Shared;

namespace MundoRuta.Server.Client.Navigation
{
    // Define los enlaces de navegación disponibles para cada rol.
    public static class MenuPorRol
    {
        private static readonly IReadOnlyList<MenuItem> MenuPublico = new List<MenuItem>
        {
            new MenuItem("Inicio", "/"),
            new MenuItem("Servicios", "/servicios"),
            new MenuItem("Prestadores", "/prestadores")
        };

        private static readonly IReadOnlyList<MenuItem> MenuUsuario = new List<MenuItem>
        {
            new MenuItem("Inicio", "/usuario"),
            new MenuItem("Solicitar viaje", "/usuario/solicitar"),
            new MenuItem("Mis viajes", "/usuario/viajes"),
            new MenuItem("Mi perfil", "/usuario/perfil")
        };

        private static readonly IReadOnlyList<MenuItem> MenuPrestador = new List<MenuItem>
        {
            new MenuItem("Panel", "/prestador"),
            new MenuItem("Solicitudes", "/prestador/solicitudes"),
            new MenuItem("Viajes", "/prestador/viajes"),
            new MenuItem("Choferes", "/prestador/choferes"),
            new MenuItem("Vehículos", "/prestador/vehiculos"),
            new MenuItem("Mi perfil", "/prestador/perfil")
        };

        private static readonly IReadOnlyList<MenuItem> MenuChofer = new List<MenuItem>
        {
            new MenuItem("Mi viaje", "/chofer"),
            new MenuItem("Historial", "/chofer/historial"),
            new MenuItem("Mi perfil", "/chofer/perfil")
        };

        private static readonly IReadOnlyList<MenuItem> MenuAdmin = new List<MenuItem>
        {
            new MenuItem("Panel", "/admin"),
            new MenuItem("Prestadores", "/admin/prestadores"),
            new MenuItem("Configuración", "/admin/configuracion")
        };

        public static IReadOnlyList<MenuItem> Obtener(string? rol)
        {
            if (string.IsNullOrWhiteSpace(rol))
            {
                return MenuPublico;
            }

            if (string.Equals(rol, Roles.Usuario, StringComparison.OrdinalIgnoreCase))
            {
                return MenuUsuario;
            }

            if (string.Equals(rol, Roles.Prestador, StringComparison.OrdinalIgnoreCase))
            {
                return MenuPrestador;
            }

            if (string.Equals(rol, Roles.Chofer, StringComparison.OrdinalIgnoreCase))
            {
                return MenuChofer;
            }

            if (string.Equals(rol, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                return MenuAdmin;
            }

            return MenuPublico;
        }
    }
}
