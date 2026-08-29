using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class ConfirmacionPagoDTO
{
    public decimal montoCobrado { get; set; }
    public string metodoPago { get; set; }
}
