using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Carga : EntidadBase
    {
        public string Tipo { get; set; }
        public int PesoEstimado { get; set; }
        public string Descripcion { get; set; }
        public int IdViaje { get; set; }

    }
}
