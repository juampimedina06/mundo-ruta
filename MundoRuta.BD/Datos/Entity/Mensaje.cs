using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Mensaje : EntidadBase
    {
        public string Contenido { get; set; }
        public string EnviadoEn { get; set; }
        public Chat Chat { get; set; }
        public Usuario Usuario { get; set; }
        public Prestador Prestador { get; set; }

    }
}
