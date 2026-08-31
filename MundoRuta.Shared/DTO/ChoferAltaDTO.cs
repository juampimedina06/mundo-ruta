using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class ChoferAltaDTO
{
    public int IdPrestador { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Licencia { get; set; }
    public required string Estado { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
