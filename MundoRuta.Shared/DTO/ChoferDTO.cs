using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class ChoferDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Licencia { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cuit { get; set; } = string.Empty;
    public int IdUsuario { get; set; }

}
