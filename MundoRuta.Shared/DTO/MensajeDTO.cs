using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class MensajeDTO
{
    public int Id { get; set; }
    public int IdChat { get; set; }
    public int? IdEmisor { get; set; }
    public string? NombreEmisor { get; set; }
    public int? IdDestinatario { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime EnviadoEn { get; set; }

}
