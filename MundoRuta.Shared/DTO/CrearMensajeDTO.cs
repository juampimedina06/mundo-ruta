using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class CrearMensajeDTO
{
    public int IdChat { get; set; }
    public int IdEmisor { get; set; }
    public int IdDestinatario { get; set; }
    public string Contenido { get; set; } = string.Empty;

}
