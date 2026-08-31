using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/pasajero")]
    public class CalificacionesController : ControllerBase
    {

        private readonly AppDbContext context;

        public CalificacionesController(AppDbContext context)
        {
            this.context = context;
        }


        [HttpPost("calificaciones")]
        public async  Task<ActionResult> Puntuacion([FromBody] CalificacionesDTO dto)
        {
            var viaje = await context.Viajes.FindAsync(dto.viajeId);

            if (viaje == null)
            {
                return NotFound("Viaje no encontrado");
            }

            if(viaje.Estado!= "FINALIZADO")
            {
                return BadRequest("El viaje tiene que estar finalizado para poder calificarlo");
            }

            var calificacion = new Calificacion
            {
                IdViaje = dto.viajeId,
                IdUsuario = dto.usuarioId,
                IdUsuarioPrestador = dto.usuarioPrestadorId,
                Puntaje = dto.puntaje,
                Comentario = dto.comentario,
            };

            context.Calificaciones.Add(calificacion);
            await context.SaveChangesAsync();

            return Ok(calificacion);


        }


        [HttpGet("api/prestador/{IdPrestador}/calificaciones")]

        public async Task<ActionResult> ListadoCalificaciones(int IdPrestador)
        {
            var lista = await context.Calificaciones.Where(c => c.IdUsuarioPrestador == IdPrestador).ToListAsync();

            double promedio = lista.Any() ? lista.Average(c => c.Puntaje) : 0.0;

            return Ok(new
            {
                promedioCalificacion = promedio,   
                totalReseñas = lista.Count,        
                comentarios = lista               
            });
        }

    }
}
