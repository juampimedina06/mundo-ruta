using System;

namespace MundoRuta.Shared.DTO
{
    public class ChoferPerfilDTO
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Licencia { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? FotoPerfil { get; set; }
        public string Empresa { get; set; } = string.Empty;
        public double PromedioCalificacion { get; set; }
        public int TotalCalificaciones { get; set; }
    }
}
