using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Calificacion : EntidadBase
    {
        public int Puntaje { get; set; }
        public string Comentario { get; set; }

        public int IdViaje { get; set; }

        [ForeignKey("IdViaje")]
        public Viaje Viaje { get; set; }

        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        public int IdUsuarioPrestador { get; set; }

        [ForeignKey("IdUsuarioPrestador")]
        public Usuario UsuarioPrestador { get; set; }
    }
}