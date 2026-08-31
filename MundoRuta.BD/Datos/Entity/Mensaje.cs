using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Mensaje : EntidadBase
    {
        public int IdChat { get; set; }
        public int? IdEmisor { get; set; }
        public int? IdDestinatario { get; set; }
        public string Contenido { get; set; }
        public DateTime EnviadoEn { get; set; }
        public Chat Chat { get; set; }
        public Usuario Emisor { get; set; }
        public Usuario Destinatario { get; set; }

    }
}
