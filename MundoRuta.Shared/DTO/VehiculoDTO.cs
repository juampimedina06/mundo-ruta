using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class VehiculoDTO
{
    public int Id { get; set; }
    public string Patente { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Licencia { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CapacidadCarga { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public int? IdChofer { get; set; }
    public string MarcaModelo { get; set; } = string.Empty;
    public DateTime? FechaFabricación { get; set; }
    public int NumeroConductor { get; set; }
    public string CaracteristicasConductor { get; set; } = string.Empty;


}
