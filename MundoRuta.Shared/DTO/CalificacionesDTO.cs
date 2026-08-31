using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MundoRuta.Shared.DTO;

public class CalificacionesDTO
{

    public int viajeId {  get; set; }
    public int usuarioId { get; set; }

    public int usuarioPrestadorId { get; set; }
    public int puntaje { get; set; }
    public string comentario { get; set; }
}
