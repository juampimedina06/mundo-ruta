using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class ResponderSolicitudDTO
{
    public required string Accion { get; set; }

    public decimal MontoEstimado { get; set; }
    public int IdChofer { get; set; }
    public  int IdVehiculo { get; set; }
}
