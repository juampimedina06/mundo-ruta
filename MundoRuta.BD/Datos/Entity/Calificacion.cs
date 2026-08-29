using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Calificacion : EntidadBase
    {
        public int Puntaje { get; set; }
        public string Comentario { get; set; }

        public int IdViaje { get; set; }
        public Viaje Viaje { get; set; }

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        public int IdPrestador { get; set; }
        public Prestador Prestador { get; set; }

    }
}
