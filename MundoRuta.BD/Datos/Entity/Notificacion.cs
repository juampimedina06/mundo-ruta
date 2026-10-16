using System;
using System.Collections.Generic;
using System.Text;


namespace MundoRuta.BD.Datos.Entity
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int? PrestadorId { get; set; }
        public PrestadorServicio? Prestador { get; set; } 
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public bool Leida { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
