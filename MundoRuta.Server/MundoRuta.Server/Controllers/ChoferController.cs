using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/chofer")]
    public class ChoferController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChoferController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{choferId}/viaje-activo")]
        public async Task<ActionResult> GetViajeActivo(int choferId)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Usuario)
                .Where(v => v.IdChofer == choferId && v.Estado != "finalizado" && v.Estado != "cancelado")
                .FirstOrDefaultAsync();

            if (viaje == null) return NotFound("El chofer no tiene viajes activos en este momento.");

            return Ok(viaje);
        }

        [HttpPut("viajes/{id}/avanzar-estado")]
        public async Task<ActionResult> AvanzarEstadoViaje(int id)
        {
            var viaje = await _context.Viajes.FindAsync(id);
            if (viaje == null) return NotFound("Viaje no encontrado.");

            string nuevoEstado = "";
            switch (viaje.Estado?.ToLower())
            {
                case "asignado":
                    nuevoEstado = "carga-subida";
                    break;
                case "carga-subida":
                    nuevoEstado = "en-transito";
                    break;
                case "en-transito":
                    nuevoEstado = "llegada-destino";
                    break;
                case "llegada-destino":
                    return BadRequest("No se puede avanzar el estado. Debe validar el DNI para finalizar la entrega.");
                default:
                    return BadRequest("El viaje tiene un estado inválido o ya fue finalizado/cancelado.");
            }

            viaje.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"El viaje avanzó exitosamente al estado: {nuevoEstado}" });
        }

        [HttpPost("viajes/{id}/validar-entrega")]
        public async Task<ActionResult> ValidarEntrega(int id, [FromBody] ValidarEntregaDTO dto)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (viaje == null) return NotFound("Viaje no encontrado.");

            if (viaje.Usuario?.Cuit != dto.CuitIngresado)
            {
                return BadRequest("DNI inválido. No se puede entregar el viaje.");
            }

            viaje.Estado = "finalizado";

            var chofer = await _context.Choferes.FindAsync(viaje.IdChofer);
            if (chofer != null)
            {
                chofer.Estado = "DISPONIBLE";
            }

            var calificacion = new Calificacion
            {
                IdViaje = viaje.Id,
                IdUsuario = viaje.IdUsuario, 
                Puntaje = dto.PuntajePasajero,
                Comentario = dto.Comentario
               
            };
            _context.Calificaciones.Add(calificacion);

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "DNI validado con éxito. Viaje finalizado." });
        }

        [HttpGet("{choferId}/historial")]
        public async Task<ActionResult> GetHistorial(int choferId)
        {
            var viajes = await _context.Viajes
                .Where(v => v.IdChofer == choferId && v.Estado == "finalizado")
                .OrderByDescending(v => v.Id)
                .ToListAsync();

            return Ok(viajes);
        }


        [HttpGet("{choferId}/perfil")]
        public async Task<ActionResult> GetPerfil(int choferId)
        {
            var chofer = await _context.Choferes.FindAsync(choferId);
            if (chofer == null) return NotFound("Chofer no encontrado.");

            var puntajes = await _context.Calificaciones
                .Include(c => c.Viaje)
                .Where(c => c.Viaje.IdChofer == choferId)
                .Select(c => c.Puntaje)
                .ToListAsync();

            double promedio = puntajes.Any() ? puntajes.Average() : 0.0;

            return Ok(new
            {
                Id = chofer.Id,
                Nombre = chofer.Nombre,
                Apellido = chofer.Apellido,
                Licencia = chofer.Licencia,
                Estado = chofer.Estado,
                Telefono = chofer.Telefono,
                FotoPerfil = chofer.FotoPerfil,
                PromedioCalificacion = Math.Round(promedio, 1) // Redondeado a 1 decimal (ej: 4.5)
            });
        }

        [HttpPut("{choferId}/perfil")]
        public async Task<ActionResult> ActualizarPerfil(int choferId, [FromBody] ActualizarPerfilDTO dto)
        {
            var chofer = await _context.Choferes.FindAsync(choferId);
            if (chofer == null) return NotFound("Chofer no encontrado.");

            if (!string.IsNullOrEmpty(dto.Nombre)) chofer.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido)) chofer.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Telefono)) chofer.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.FotoPerfil)) chofer.FotoPerfil = dto.FotoPerfil;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Perfil actualizado correctamente." });
        }
    }
}