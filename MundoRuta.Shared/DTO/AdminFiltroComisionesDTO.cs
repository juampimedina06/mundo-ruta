using System;

namespace MundoRuta.Shared.DTO
{
    public class AdminFiltroComisionesDTO
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? PrestadorId { get; set; }
        public string? EstadoViaje { get; set; }
    }
}
