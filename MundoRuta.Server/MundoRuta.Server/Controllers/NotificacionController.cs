using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos; 
using MundoRuta.BD.Datos.Entity;
using System;
using System.Threading.Tasks;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    public class NotificacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificacionController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult> CrearNotificacion([FromBody] CrearNotificacionDTO dto)
        {
            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                PrestadorId = dto.PrestadorId,
                Titulo = dto.Titulo,
                Mensaje = dto.Mensaje,
                Tipo = dto.Tipo,
                Leida = false,
                FechaCreacion = DateTime.Now
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Notificación creada con éxito" });
        }

       
        [HttpPost("viaje/{viajeId}/asignado")]
        public async Task<ActionResult> NotificarViajeAsignado(int viajeId)
        {
            
            var viaje = await _context.Viajes.FindAsync(viajeId);

            if (viaje == null)
            {
                return NotFound(new { mensaje = "Viaje no encontrado" });
            }

            var notificacion = new Notificacion
            {
                UsuarioId = viaje.IdUsuario,
                Titulo = "Viaje Asignado",
                Mensaje = "Un prestador ha aceptado tu viaje",
                Leida = false,
                FechaCreacion = DateTime.Now
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Notificación de asignación enviada" });
        }
    }
}