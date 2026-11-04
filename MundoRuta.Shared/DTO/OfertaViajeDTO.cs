using System;

namespace MundoRuta.Shared.DTO
{
    public class OfertaViajeDTO
    {
        public int IdOferta { get; set; }
        public int IdPrestador { get; set; }
        public string NombrePrestador { get; set; }
        public decimal MontoOfertado { get; set; }
        public double PromedioCalificacionPrestador { get; set; }
        public bool EsMasBarata { get; set; }
        public bool EsMejorCalificado { get; set; }
        public bool EsContraoferta { get; set; }
        public string EstadoOferta { get; set; }
        public DateTime FechaOferta { get; set; }
    }
}
