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
        private readonly AppDbContext _context;

        public PasajeroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPut("viajes/{id:int}/cancelar")]
        public async Task<IActionResult> CancelarViaje(int id, [FromBody] CancelarViajeDTO dto)
        {
            var viaje = await _context.Viajes.FindAsync(id);

            if (viaje == null)
            {
                return NotFound("El viaje no existe");
            }

            var fechaHoraViaje = viaje.Fecha.Date + viaje.Hora;
            var tiempoRestante = fechaHoraViaje - DateTime.Now;

            if (tiempoRestante.TotalMinutes < 30)
            {
                return BadRequest("No se puede cancelar el viaje con menos de 30 minutos de anticipación");
            }

            viaje.Estado = "CANCELADO";

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Viaje cancelado con éxito", motivo = dto.Motivo });
        }

        [HttpPost("viajes/solicitar")]
        public async Task<ActionResult> SolicitarViaje([FromBody] SolicitarViajeDTO dto)
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
                IdUsuario = dto.IdUsuario,
                IdServicio = dto.IdServicio
            };

            _context.Viajes.Add(viaje);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud de viaje creada", id = viaje.Id });
        }

        [HttpPut("viajes/{id:int}/responder-contraoferta")]
        public async Task<IActionResult> ResponderContraOferta(int id, [FromBody] ResponderContraOfertaDTO dto)
        {
            var viaje = await _context.Viajes.FindAsync(id);

            if (viaje == null)
            {
                return NotFound("El viaje no existe");
            }

            if (dto.Aceptar)
            {
                viaje.Estado = "ACEPTADO";
            }
            else
            {
                viaje.Estado = "CANCELADO";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = dto.Aceptar
                    ? "Contraoferta aceptada"
                    : "Contraoferta rechazada"
            });
        }
    }
}
