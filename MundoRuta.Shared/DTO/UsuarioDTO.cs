using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class UsuarioDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; }
        public string Rol { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }

    }
}
