using System;

namespace MundoRuta.Shared.DTO.Realtime
{
    public class EstadoViajeActualizadoDTO
    {
        public int IdViaje { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Mensaje { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}
