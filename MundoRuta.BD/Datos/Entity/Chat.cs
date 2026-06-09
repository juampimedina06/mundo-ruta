using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Chat : EntidadBase
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public Viaje IdViaje { get; set; }


    }
}
