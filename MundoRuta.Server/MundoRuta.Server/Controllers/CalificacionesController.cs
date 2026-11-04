using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class CalificacionesController : ControllerBase
    {
        private readonly ICalificacionRepositorio _calificacionRepositorio;
        private readonly IViajeRepositorio _viajeRepositorio;

        public CalificacionesController(
            ICalificacionRepositorio calificacionRepositorio,
            IViajeRepositorio viajeRepositorio)
        {
            _calificacionRepositorio = calificacionRepositorio;
            _viajeRepositorio = viajeRepositorio;
        }

        // POST: Calificar viajes
        [HttpPost("pasajero/calificaciones")]
        public async Task<ActionResult<CalificacionesResponseDTO>> Puntuacion(CalificacionesDTO dto)
        {
            var viaje = await _viajeRepositorio.ObtenerPorId(dto.viajeId);

            if (viaje == null)
                return NotFound("Viaje no encontrado");

            if (viaje.Estado != "FINALIZADO")
                return BadRequest("El viaje tiene que estar finalizado para poder calificarlo");

            // 1. EL BLOQUEO: Evitamos que califiquen dos veces el mismo viaje
            if (viaje.CalificadoPorPasajero)
                return BadRequest("Este viaje ya fue calificado.");

            var calificacion = new Calificacion
            {
                IdViaje = dto.viajeId,
                IdUsuario = dto.usuarioId,
                IdUsuarioPrestador = dto.usuarioPrestadorId,
                Puntaje = dto.puntaje,
                Comentario = dto.comentario,
            };

            var creada = await _calificacionRepositorio.CrearCalificacion(calificacion);

            // 2. LA MAGIA: Cambiamos el estado del viaje para desbloquear el detalle al pasajero
            viaje.CalificadoPorPasajero = true;

            // Guardamos el cambio usando tu repositorio (Ajustá el nombre del método si en tu interfaz se llama Actualizar, UpdateAsync, etc.)
            await _viajeRepositorio.Update(viaje);

            var response = new CalificacionesResponseDTO
            {
                id = creada.Id,
                viajeId = creada.IdViaje,
                usuarioId = creada.IdUsuario,
                usuarioPrestadorId = creada.IdUsuarioPrestador,
                puntaje = creada.Puntaje,
                comentario = creada.Comentario
            };

            return Ok(response);
        }

        // GET: Listado de calificaciones de un prestador
        [HttpGet("prestador/{IdPrestador:int}/calificaciones")]
        public async Task<ActionResult<ListadoCalificacionesDTO>> ListadoCalificaciones(int IdPrestador)
        {
            var lista = await _calificacionRepositorio.ObtenerPorPrestador(IdPrestador);

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