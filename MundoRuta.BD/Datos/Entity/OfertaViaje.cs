using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class OfertaViaje : EntidadBase
    {
        public int IdViaje { get; set; }
        public int IdPrestador { get; set; }
        public int? IdChoferPropuesto { get; set; }
        public decimal MontoOfertado { get; set; }
        public bool EsContraoferta { get; set; }
        public string EstadoOferta { get; set; }
        public DateTime FechaOferta { get; set; }

        public Viaje Viaje { get; set; }
        public Usuario Prestador { get; set; }
        public Chofer ChoferPropuesto { get; set; }

    }
}
