using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class ResponderSolicitudDTO
{
    public required string Accion { get; set; }
    public int idChofer { get; set; }
    public int idVehiculo { get; set; }
}
