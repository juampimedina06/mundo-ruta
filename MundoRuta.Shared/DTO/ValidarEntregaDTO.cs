using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class ValidarEntregaDTO
    {
        public string CuitIngresado { get; set; } = string.Empty;
        public int PuntajePasajero { get; set; }
        public string? Comentario { get; set; }
    }
}
