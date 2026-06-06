using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class PrestadorServicio : EntidadBase
    {
        public int idPrestador { get; set; }
        public Prestador Prestador { get; set; }
        public int idServicio { get; set; }
        public Servicio Servicio { get; set; } 
    }
}
