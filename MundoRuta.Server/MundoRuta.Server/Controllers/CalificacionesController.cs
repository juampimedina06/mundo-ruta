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

        //Post
        //Calificar viajes
        [HttpPost("calificaciones")]
        public async Task<ActionResult<CalificacionesResponseDTO>> Puntuacion(CalificacionesDTO dto)
        {
            var viaje = await context.Viajes.FindAsync(dto.viajeId);

            if (viaje == null)
                return NotFound("Viaje no encontrado");

            if (viaje.Estado != "FINALIZADO")
                return BadRequest("El viaje tiene que estar finalizado para poder calificarlo");

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

            var response = new CalificacionesResponseDTO
            {
                id = calificacion.Id,
                viajeId = calificacion.IdViaje,
                usuarioId = calificacion.IdUsuario,
                usuarioPrestadorId = calificacion.IdUsuarioPrestador,
                puntaje = calificacion.Puntaje,
                comentario = calificacion.Comentario
            };

            return Ok(response);
        }

        //Get
        //Listado de calificaciones
        [HttpGet("api/prestador/{IdPrestador:int}/calificaciones")]

        public async Task<ActionResult<ListadoCalificacionesDTO>> ListadoCalificaciones(int IdPrestador)
        {
            var lista = await context.Calificaciones.Where(c => c.IdUsuarioPrestador == IdPrestador).ToListAsync();

            var response = new ListadoCalificacionesDTO
            {
                promedioCalificacion = lista.Any() ? lista.Average(c => c.Puntaje) : 0.0,
                totalReseñas = lista.Count,
                comentarios = lista.Select(c => new CalificacionItemDTO
                {
                    puntaje = c.Puntaje,
                    comentario = c.Comentario,
                    usuarioId = c.IdUsuario
                }).ToList()
            };

            return Ok(response);


        }
    }
}
