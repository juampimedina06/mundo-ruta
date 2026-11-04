using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [Route("api/chofer")]
    [ApiController]
    public class ChoferController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChoferController(AppDbContext context)
        {
            _context = context;
        }

        // GET {choferId}/viaje-activo: devuelve el viaje actual en curso
        [HttpGet("{choferId}/viaje-activo")]
        public async Task<ActionResult> GetViajeActivo(int choferId)
        {
            var estadosActivos = new[] { "Asignado", "Carga-subida", "En-transito", "Llegada-destino" };

            var viaje = await _context.Viajes
                .FirstOrDefaultAsync(v => v.IdChofer == choferId && estadosActivos.Contains(v.Estado));

            if (viaje == null) return NotFound("No hay ningún viaje activo para este chofer.");

            return Ok(viaje);
        }

        // PUT viajes/{id}/avanzar-estado: Máquina de estados secuencial
        [HttpPut("viajes/{id}/avanzar-estado")]
        public async Task<ActionResult> AvanzarEstadoViaje(int id)
        {
            var viaje = await _context.Viajes.FindAsync(id);
            if (viaje == null) return NotFound("Viaje no encontrado.");

            switch (viaje.Estado)
            {
                case "Asignado":
                    viaje.Estado = "Carga-subida";
                    break;
                case "Carga-subida":
                    viaje.Estado = "En-transito";
                    break;
                case "En-transito":
                    viaje.Estado = "Llegada-destino";
                    break;
                case "Llegada-destino":
                    return BadRequest("El viaje ya llegó a destino. El siguiente paso es validar la entrega.");
                default:
                    return BadRequest($"Estado actual '{viaje.Estado}' no es válido para avanzar.");
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Estado avanzado correctamente a: {viaje.Estado}" });
        }

        // POST viajes/{id}/validar-entrega: Valida DNI y finaliza el viaje
        [HttpPost("viajes/{id}/validar-entrega")]
        public async Task<ActionResult> ValidarEntrega(int id, [FromBody] ValidarEntregaDTO dto)
        {

            var viaje = await _context.Viajes
                .Include(v => v.Chofer)
                .Include(v => v.Usuario) 
                .FirstOrDefaultAsync(v => v.Id == id);

            if (viaje == null) return NotFound("Viaje no encontrado.");


            if (viaje.Usuario.Cuit != dto.CuitIngresado)
            {
                return BadRequest("DNI inválido.");
            }


            viaje.Estado = "Finalizado";
            if (viaje.Chofer != null) viaje.Chofer.Estado = "DISPONIBLE";


            var calificacion = new Calificacion
            {
                IdViaje = viaje.Id,
                Puntaje = dto.PuntajePasajero,
                Comentario = dto.Comentario
            };
            _context.Calificaciones.Add(calificacion);

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Entrega validada exitosamente. Viaje finalizado." });
        }

        // GET {choferId}/historial: lista viajes completados
        [HttpGet("{choferId}/historial")]
        public async Task<ActionResult> GetHistorial(int choferId)
        {
            var viajesCompletados = await _context.Viajes
                .Where(v => v.IdChofer == choferId && v.Estado == "Finalizado")
                .OrderByDescending(v => v.Id)
                .ToListAsync();

            return Ok(viajesCompletados);
        }

        // GET {choferId}/perfil: obtiene datos, foto y promedio
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
                chofer.Id,
                chofer.Nombre,
                chofer.Apellido,
                chofer.Licencia,
                chofer.Estado,
                chofer.Telefono,
                chofer.FotoPerfil,
                PromedioCalificacion = Math.Round(promedio, 1)
            });
        }

        // PUT {choferId}/perfil: actualiza datos personales de forma segura
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