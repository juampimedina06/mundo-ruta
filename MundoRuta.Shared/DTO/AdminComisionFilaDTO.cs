using System;

namespace MundoRuta.Shared.DTO
{
    public class AdminComisionFilaDTO
    {
        public int ViajeId { get; set; }
        public int PrestadorId { get; set; }
        public string PrestadorNombre { get; set; } = string.Empty;
        public string? PrestadorRazonSocial { get; set; }
        public int PasajeroId { get; set; }
        public string PasajeroNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal PorcentajeComision { get; set; }
        public decimal MontoComision { get; set; }
        public string EstadoViaje { get; set; } = string.Empty;
        public string EstadoPago { get; set; } = string.Empty;
    }
}
