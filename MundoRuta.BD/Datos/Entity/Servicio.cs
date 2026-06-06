using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Servicio : EntidadBase
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }

    }
}
