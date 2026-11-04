using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class ChoferUpdateDto
    {
        public string Telefono { get; set; } = string.Empty;
        public string Licencia { get; set; } = string.Empty;
        public string EstadoOperativo { get; set; } = string.Empty;
    }
}
