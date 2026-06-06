using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Liquidacion : EntidadBase
    {
        public int MontoBruto { get; set; }
        public int Comision { get; set; }
        public int MontoNeto { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
        public int idPrestador { get; set; }
        public Prestador Prestador { get; set; }
    }
}
