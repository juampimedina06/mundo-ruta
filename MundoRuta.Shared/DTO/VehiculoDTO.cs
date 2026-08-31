using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class VehiculoDTO
{
    public int Id { get; set; }
    public string Patente { get; set; }
    public string Marca { get; set; }
    public string Licencia { get; set; }
    public string Estado { get; set; }
    public string CapacidadCarga { get; set; }
    public string TipoVehiculo { get; set; }
    public int IdUsuario { get; set; }
    public int IdChofer { get; set; }

}
