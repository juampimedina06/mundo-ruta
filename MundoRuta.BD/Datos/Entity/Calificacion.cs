using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Calificacion : EntidadBase
    {
        public int Puntaje { get; set; }
        public string Comentario { get; set; }
        public Viaje Viaje { get; set; }
        public Usuario Usuario { get; set; }
        public Prestador Prestador { get; set; }

    }
}
