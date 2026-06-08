using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Pago : EntidadBase
    {
        public int Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; }
        public Boolean Estado { get; set; }
        public int IdViaje { get; set; }
    }
}
