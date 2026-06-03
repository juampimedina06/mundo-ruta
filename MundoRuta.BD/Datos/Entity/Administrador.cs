using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos.Entity
{
    public class Administrador : EntidadBase
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int Telefono { get; set; }
  
    }
}
