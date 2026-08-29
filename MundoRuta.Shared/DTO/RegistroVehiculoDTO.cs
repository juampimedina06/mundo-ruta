using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class RegistroVehiculoDTO
    {
        public string Patente { get; set; }
        public string Marca { get; set; }
        public string Licencia { get; set; }
        public string Estado { get; set; }
        public DateTime FechaFabricación { get; set; }
        public int NumeroConductor { get; set; }
        public string CaracteristicasConductor { get; set; }
        public string TipoVehiculo { get; set; }
        public string CapacidadCarga { get; set; }
        public string MarcaModelo { get; set; }
        public int IdPrestador { get; set; }
        public int IdChofer { get; set; }

    }
}
