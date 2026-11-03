using System.Collections.Generic;

namespace MundoRuta.Shared.DTO
{
    public class AdminComisionesReporteDTO
    {
        public decimal TotalFacturado { get; set; }
        public decimal TotalComisionesRetenidas { get; set; }
        public decimal TotalComisionesPendientes { get; set; }
        public int CantidadOperaciones { get; set; }
        public List<AdminComisionFilaDTO> Operaciones { get; set; } = new List<AdminComisionFilaDTO>();
    }
}
