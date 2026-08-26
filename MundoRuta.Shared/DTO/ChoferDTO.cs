using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class ChoferDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Licencia { get; set; }
    public bool Estado { get; set; }
    public int IdPrestador { get; set; }

}
