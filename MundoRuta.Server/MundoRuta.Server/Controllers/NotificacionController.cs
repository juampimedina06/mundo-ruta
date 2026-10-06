using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos; // Asegurate de que este sea el namespace de tu AppDbContext
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

        // 1. POST: Crear una notificación general
        [HttpPost]
        public async Task<IActionResult> CrearNotificacion([FromBody] CrearNotificacionDTO dto)
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

        // 2. POST: Trigger automático cuando se asigna un prestador a un viaje
        [HttpPost("viaje/{viajeId}/asignado")]
        public async Task<IActionResult> NotificarViajeAsignado(int viajeId)
        {
            // Se corrigió pedidoId por viajeId para que coincida con el parámetro
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