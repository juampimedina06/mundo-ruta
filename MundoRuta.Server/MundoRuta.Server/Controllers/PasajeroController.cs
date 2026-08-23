using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos;
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

        [HttpPut("viajes/{id}/cancelar")]
        public async Task<IActionResult> CancelarViaje(int id, [FromBody] CancelarViajeDTO dto)
        {
            var viaje = await context.Viajes.FindAsync(id);

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

            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Viaje cancelado con éxito",
                motivo = dto.Motivo
            });
        }
    }
}
