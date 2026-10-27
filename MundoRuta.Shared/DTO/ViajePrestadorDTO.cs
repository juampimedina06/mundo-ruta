using System;

namespace MundoRuta.Shared.DTO;

public class ViajePrestadorDTO
{
    public int Id { get; set; }

    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;

    public string TipoSolicitud { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;
    public string EstadoPago { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }

    public decimal Monto { get; set; }
    public decimal MontoEstimado { get; set; }

    public int IdPasajero { get; set; }
    public string NombrePasajero { get; set; } = string.Empty;
}