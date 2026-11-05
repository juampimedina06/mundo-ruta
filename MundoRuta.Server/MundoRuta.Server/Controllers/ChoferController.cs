using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Server.Hubs;
using MundoRuta.Shared;
using MundoRuta.Shared.DTO;
using MundoRuta.Shared.DTO.Realtime;

namespace MundoRuta.Server.Controllers
{
    [Route("api/chofer")]
    [ApiController]
    public class ChoferController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ViajeHub> _hubContext;

        public ChoferController(AppDbContext context, IHubContext<ViajeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET {choferId}/viaje-activo: devuelve el viaje actual en curso
        [HttpGet("{choferId}/viaje-activo")]
        public async Task<ActionResult> GetViajeActivo(int choferId)
        {
            string[] estadosActivos = EstadosViaje.Activos;

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

            switch (EstadosViaje.Normalizar(viaje.Estado))
            {
                case "asignado":
                    viaje.Estado = EstadosViaje.CargaSubida;
                    break;
                case "carga-subida":
                    viaje.Estado = EstadosViaje.EnTransito;
                    break;
                case "en-transito":
                    viaje.Estado = EstadosViaje.LlegadaDestino;
                    break;
                case "llegada-destino":
                    return BadRequest("El viaje ya llegó a destino. El siguiente paso es validar la entrega.");
                default:
                    return BadRequest($"Estado actual '{viaje.Estado}' no es válido para avanzar.");
            }

            await _context.SaveChangesAsync();
            await NotificarEstadoAsync(viaje, $"Estado avanzado correctamente a: {viaje.Estado}");
            return Ok(new { mensaje = $"Estado avanzado correctamente a: {viaje.Estado}" });
        }

        // POST viajes/{id}/validar-entrega: Valida DNI y finaliza el viaje
        [HttpPost("viajes/{id}/validar-entrega")]
        public async Task<ActionResult> ValidarEntrega(int id, [FromBody] ValidarEntregaDTO dto)
        {

            var viaje = await _context.Viajes
                .Include(v => v.Chofer)
                .Include(v => v.Usuario)
                .Include(v => v.Pasajero)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (viaje == null) return NotFound("Viaje no encontrado.");


            if (viaje.Usuario.Cuit != dto.CuitIngresado)
            {
                return BadRequest("DNI inválido.");
            }


            viaje.Estado = EstadosViaje.Finalizado;
            if (viaje.Chofer != null) viaje.Chofer.Estado = "DISPONIBLE";


            var calificacion = new Calificacion
            {
                IdViaje = viaje.Id,
                IdUsuario = viaje.IdPasajero,
                IdUsuarioPrestador = viaje.IdUsuario,
                Viaje = viaje,
                Usuario = viaje.Pasajero,
                UsuarioPrestador = viaje.Usuario,
                Puntaje = dto.PuntajePasajero,
                Comentario = dto.Comentario ?? string.Empty
            };
            _context.Calificaciones.Add(calificacion);

            await _context.SaveChangesAsync();
            await NotificarEstadoAsync(viaje, "Entrega validada exitosamente. Viaje finalizado.");
            return Ok(new { mensaje = "Entrega validada exitosamente. Viaje finalizado." });
        }

        // GET {choferId}/historial: lista viajes completados
        [HttpGet("{choferId}/historial")]
        public async Task<ActionResult> GetHistorial(int choferId)
        {
            string[] estadosFinalizados = EstadosViaje.Finalizados;

            var viajesCompletados = await _context.Viajes
                .Where(v => v.IdChofer == choferId && estadosFinalizados.Contains(v.Estado))
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

        private async Task NotificarEstadoAsync(Viaje viaje, string mensaje)
        {
            var estadoDto = new EstadoViajeActualizadoDTO
            {
                IdViaje = viaje.Id,
                Estado = viaje.Estado,
                Mensaje = mensaje
            };

            await _hubContext.Clients.Group($"viaje_{viaje.Id}").SendAsync("EstadoViajeActualizado", estadoDto);

            if (viaje.IdUsuario > 0)
            {
                await _hubContext.Clients.Group($"prestador_{viaje.IdUsuario}").SendAsync("EstadoViajeActualizado", estadoDto);
            }

            if (viaje.IdPasajero > 0)
            {
                await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("EstadoViajeActualizado", estadoDto);
            }

            if (viaje.IdChofer > 0)
            {
                await _hubContext.Clients.Group($"chofer_{viaje.IdChofer}").SendAsync("EstadoViajeActualizado", estadoDto);
            }
        }
    }
}