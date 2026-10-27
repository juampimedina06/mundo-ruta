using System;

namespace MundoRuta.Shared.DTO.Realtime
{
    public class ViajeAsignadoDTO
    {
        public int IdViaje { get; set; }
        public int IdPrestador { get; set; }
        public int? IdChofer { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}
