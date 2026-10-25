using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Chat : EntidadBase
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
        public int IdViaje { get; set; }  // Clave foránea que identifica el viaje asociado al chat.
        public Viaje Viaje { get; set; } // Propiedad de navegación hacia el viaje asociado.


    }
}
