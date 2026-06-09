using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Mensaje : EntidadBase
    {
        public int IdChat { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdPrestador { get; set; }
        public string Contenido { get; set; }
        public DateTime EnviadoEn { get; set; }
        public Chat Chat { get; set; }
        public Usuario Usuario { get; set; }
        public Prestador Prestador { get; set; }

    }
}
