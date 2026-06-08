using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Vehiculo : EntidadBase
    {
        public string Patente { get; set; }
        public string Marca { get; set; }
        public string Licencia { get; set; }
        public string Estado { get; set; }
        public DateTime FechaFabricación { get; set; }
        public int NumeroConductor { get; set; }
        public string CaracteristicasConductor { get; set; }
        public int IdChofer { get; set; }
        public Chofer Chofer { get; set; }

    }
}
