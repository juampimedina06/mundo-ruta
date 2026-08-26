using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Shared.DTO
{
    public class SolicitarViajeDTO
    {
        public required string Origen { get; set; }
        public required string Destino { get; set; }
        public required string TipoSolicitud { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public bool EquipajeCarga { get; set; }
        public decimal Monto { get; set; }
        public int IdSolicitante { get; set; }
        public int IdPasajero { get; set; }
        public int IdPrestador { get; set; }
        public int IdServicio { get; set; }

    }
}
