using System;

namespace MundoRuta.Shared.DTO.Realtime
{
    public class NuevaOfertaDTO
    {
        public int IdOferta { get; set; }
        public int IdViaje { get; set; }
        public int IdPrestador { get; set; }
        public string NombrePrestador { get; set; } = string.Empty;
        public decimal MontoOfertado { get; set; }
        public bool EsContraoferta { get; set; }
        public string EstadoOferta { get; set; } = string.Empty;
        public DateTime FechaOferta { get; set; }
        public int? IdChoferPropuesto { get; set; }
    }
}
