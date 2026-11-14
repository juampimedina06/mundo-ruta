using System;

namespace MundoRuta.Shared.DTO
{
    public class ChoferViajeHistorialDTO
    {
        public int Id { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string FechaFormateada { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
