using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{ 
    [ApiController]
    [Route("api/pasajero")]
    public class PasajeroController : ControllerBase
    {
        private readonly AppDbContext context;

        public PasajeroController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost("viajes/solicitar")]
        public async Task<IActionResult> SolicitarViaje([FromBody] SolicitarViajeDTO dto)
        {
            var viaje = new Viaje
            {
                Origen = dto.Origen,
                Destino = dto.Destino,
                TipoSolicitud = dto.TipoSolicitud,
                Estado = "PENDIENTE",
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                EquipajeCarga = dto.EquipajeCarga,
                Monto = dto.Monto,
                IdSolicitante = dto.IdSolicitante,
                IdPasajero = dto.IdPasajero,
                IdPrestador = dto.IdPrestador,
                IdServicio = dto.IdServicio
            };

            context.Viajes.Add(viaje);
            await context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud de viaje creada", id = viaje.Id });
        }
    }
}
