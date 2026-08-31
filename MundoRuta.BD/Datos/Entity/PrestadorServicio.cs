using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class PrestadorServicio : EntidadBase
    {
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public int idServicio { get; set; }
        public Servicio Servicio { get; set; } 
    }
}
