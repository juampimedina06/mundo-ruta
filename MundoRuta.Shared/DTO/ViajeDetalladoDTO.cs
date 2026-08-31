using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class ViajeDetalladoDTO
    {
        public int Id { get; set; }

        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;

        public string TipoSolicitud { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
        public string EstadoPago { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public DateTime FechaHoraReserva { get; set; }

        public decimal Monto { get; set; }
        public decimal MontoEstimado { get; set; }

        public string MetodoDePago { get; set; } = string.Empty;

        public bool EquipajeCarga { get; set; }
        public string? DetalleCarga { get; set; }

        public string? MotivoCancelacion { get; set; }

        // Pasajero
        public int IdPasajero { get; set; }
        public string NombrePasajero { get; set; } = string.Empty;

        // Chofer
        public int? IdChofer { get; set; }
        public string? NombreChofer { get; set; }

        // Vehículo
        public int? IdVehiculo { get; set; }
        public string? Patente { get; set; }
        public string? Marca { get; set; }
    }
}
