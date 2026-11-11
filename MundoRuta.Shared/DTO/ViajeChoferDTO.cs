using System;

namespace MundoRuta.Shared.DTO
{
    public class ViajeChoferDTO
    {
        public int Id { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public decimal Monto { get; set; }
        public string MetodoDePago { get; set; } = string.Empty;
        public string? DetalleCarga { get; set; }
        public bool EsCubierto { get; set; }
        public bool UsaBaulera { get; set; }
        public bool LlevaAcompanante { get; set; }
        public string DniReceptor { get; set; } = string.Empty;
        public int IdPasajero { get; set; }
        public string NombrePasajero { get; set; } = string.Empty;
        public string? TelefonoPasajero { get; set; }
        public int IdChofer { get; set; }
        public int IdPrestador { get; set; }
        public string? NombrePrestador { get; set; }
    }
}
