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

        private async Task<Chofer?> ResolverChoferAsync(int id)
        {
            return await _context.Choferes
                .Include(c => c.Prestador)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id || c.IdUsuario == id);
        }

        // GET {choferId}/viaje-activo: devuelve el viaje actual en curso
        [HttpGet("{choferId}/viaje-activo")]
        public async Task<ActionResult<ViajeChoferDTO>> GetViajeActivo(int choferId)
        {
            Chofer? chofer = await ResolverChoferAsync(choferId);
            if (chofer == null)
            {
                return NotFound("Chofer no encontrado.");
            }

            string[] estadosActivos = EstadosViaje.Activos;

            var viaje = await _context.Viajes
                .Include(v => v.Pasajero)
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(v => v.IdChofer == chofer.Id && estadosActivos.Contains(v.Estado));

            if (viaje == null)
            {
                return NotFound("No hay ningún viaje activo para este chofer.");
            }

            string nombrePrestador = viaje.Usuario != null
                ? (string.IsNullOrWhiteSpace(viaje.Usuario.RazonSocial)
                    ? $"{viaje.Usuario.Nombre} {viaje.Usuario.Apellido}".Trim()
                    : viaje.Usuario.RazonSocial)
                : string.Empty;

            var dto = new ViajeChoferDTO
            {
                Id = viaje.Id,
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                Estado = viaje.Estado,
                Fecha = viaje.Fecha,
                Hora = viaje.Hora,
                Monto = viaje.Monto,
                MetodoDePago = viaje.MetodoDePago ?? string.Empty,
                DetalleCarga = viaje.DetalleCarga,
                EsCubierto = viaje.EsCubierto,
                UsaBaulera = viaje.UsaBaulera,
                LlevaAcompanante = viaje.LlevaAcompanante,
                DniReceptor = viaje.DniReceptor ?? string.Empty,
                IdPasajero = viaje.IdPasajero,
                NombrePasajero = viaje.Pasajero != null
                    ? $"{viaje.Pasajero.Nombre} {viaje.Pasajero.Apellido}".Trim()
                    : string.Empty,
                TelefonoPasajero = viaje.Pasajero?.Telefono,
                IdChofer = chofer.Id,
                IdPrestador = viaje.IdUsuario,
                NombrePrestador = nombrePrestador
            };

            return Ok(dto);
        }

        // PUT viajes/{id}/avanzar-estado: Máquina de estados secuencial
        [HttpPut("viajes/{id}/avanzar-estado")]
        public async Task<ActionResult> AvanzarEstadoViaje(int id)
        {
            Viaje? viaje = await _context.Viajes.FindAsync(id);
            if (viaje == null)
            {
                return NotFound("Viaje no encontrado.");
            }

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
            return Ok(new { mensaje = $"Estado avanzado correctamente a: {viaje.Estado}", estado = viaje.Estado });
        }

        // POST viajes/{id}/validar-entrega: Valida DNI y finaliza el viaje
        [HttpPost("viajes/{id}/validar-entrega")]
        public async Task<ActionResult> ValidarEntrega(int id, [FromBody] ValidarEntregaDTO dto)
        {
            Viaje? viaje = await _context.Viajes
                .Include(v => v.Chofer)
                .Include(v => v.Usuario)
                .Include(v => v.Pasajero)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (viaje == null)
            {
                return NotFound("Viaje no encontrado.");
            }

            string dniEsperado = (viaje.DniReceptor ?? string.Empty).Trim();
            string dniRecibido = (dto.DniIngresado ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(dniRecibido) || !string.Equals(dniEsperado, dniRecibido, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("DNI inválido.");
            }

            viaje.Estado = EstadosViaje.Finalizado;
            viaje.CalificadoPorChofer = true;

            if (viaje.Chofer != null)
            {
                viaje.Chofer.Estado = "DISPONIBLE";
            }

            if (dto.PuntajePasajero > 0)
            {
                int emisorCalificacionId = viaje.Chofer != null && viaje.Chofer.IdUsuario > 0
                    ? viaje.Chofer.IdUsuario
                    : viaje.IdUsuario;

                var calificacion = new Calificacion
                {
                    IdViaje = viaje.Id,
                    IdUsuario = emisorCalificacionId,
                    IdUsuarioPrestador = viaje.IdPasajero,
                    Viaje = viaje,
                    Puntaje = Math.Clamp(dto.PuntajePasajero, 1, 5),
                    Comentario = dto.Comentario ?? string.Empty
                };
                _context.Calificaciones.Add(calificacion);
            }

            await _context.SaveChangesAsync();
            await NotificarEstadoAsync(viaje, "Entrega validada exitosamente. Viaje finalizado.");
            return Ok(new { mensaje = "Entrega validada exitosamente. Viaje finalizado." });
        }

        // GET {choferId}/historial: lista viajes completados
        [HttpGet("{choferId}/historial")]
        public async Task<ActionResult<List<ChoferViajeHistorialDTO>>> GetHistorial(int choferId)
        {
            Chofer? chofer = await ResolverChoferAsync(choferId);
            if (chofer == null)
            {
                return NotFound("Chofer no encontrado.");
            }

            string[] estadosFinalizados = EstadosViaje.Finalizados;

            var viajesCompletados = await _context.Viajes
                .Where(v => v.IdChofer == chofer.Id && estadosFinalizados.Contains(v.Estado))
                .OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id)
                .Select(v => new ChoferViajeHistorialDTO
                {
                    Id = v.Id,
                    Origen = v.Origen,
                    Destino = v.Destino,
                    Monto = v.Monto,
                    FechaFormateada = v.Fecha.ToString("dd/MM/yyyy"),
                    Estado = v.Estado
                })
                .ToListAsync();

            return Ok(viajesCompletados);
        }

        // GET {choferId}/perfil: obtiene datos, foto, empresa y promedio
        [HttpGet("{choferId}/perfil")]
        public async Task<ActionResult<ChoferPerfilDTO>> GetPerfil(int choferId)
        {
            Chofer? chofer = await ResolverChoferAsync(choferId);
            if (chofer == null)
            {
                return NotFound("Chofer no encontrado.");
            }

            var puntajes = await _context.Calificaciones
                .Include(c => c.Viaje)
                .Where(c => c.Viaje.IdChofer == chofer.Id)
                .Select(c => c.Puntaje)
                .ToListAsync();

            double promedio = puntajes.Count > 0 ? puntajes.Average() : 0.0;

            string empresa = chofer.Prestador != null
                ? (string.IsNullOrWhiteSpace(chofer.Prestador.RazonSocial)
                    ? $"{chofer.Prestador.Nombre} {chofer.Prestador.Apellido}".Trim()
                    : chofer.Prestador.RazonSocial)
                : "Independiente";

            var perfilDto = new ChoferPerfilDTO
            {
                Id = chofer.Id,
                IdUsuario = chofer.IdUsuario,
                Nombre = chofer.Nombre,
                Apellido = chofer.Apellido,
                Licencia = chofer.Licencia,
                Estado = chofer.Estado,
                Telefono = chofer.Telefono,
                FotoPerfil = chofer.FotoPerfil,
                Empresa = empresa,
                PromedioCalificacion = Math.Round(promedio, 1),
                TotalCalificaciones = puntajes.Count
            };

            return Ok(perfilDto);
        }

        // PUT {choferId}/perfil: actualiza datos personales de forma segura
        [HttpPut("{choferId}/perfil")]
        public async Task<ActionResult> ActualizarPerfil(int choferId, [FromBody] ActualizarPerfilDTO dto)
        {
            Chofer? chofer = await ResolverChoferAsync(choferId);
            if (chofer == null)
            {
                return NotFound("Chofer no encontrado.");
            }

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
