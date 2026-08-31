using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity;

public class Chofer : EntidadBase
{
    public required string Nombre { get; set; }
    public required string Apellido{ get; set; }
    public required string Licencia { get; set; }
    public required string Estado { get; set; }
    public int IdUsuario { get; set; }
    public Usuario Usuario { get; set; }

}
