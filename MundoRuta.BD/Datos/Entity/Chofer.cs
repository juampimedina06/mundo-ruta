using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity;

public class Chofer : EntidadBase
{
    public required string Nombre { get; set; }
    public required string Apellido{ get; set; }
    public required string Licencia { get; set; }
    public required bool Estado { get; set; }
    public int IdPrestador { get; set; }
    public Prestador Prestador { get; set; }

}
