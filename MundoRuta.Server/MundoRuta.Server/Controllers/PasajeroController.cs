using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/pasajero")]
    public class PasajeroController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PasajeroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPut("viajes/{id:int}/cancelar")]
        public async Task<IActionResult> CancelarViaje(int id,CancelarViajeDTO dto)
        {
            var viaje = await _context.Viajes.FindAsync(id);

            if (viaje == null)
            {
                return NotFound("El viaje no existe");
            }

            if (viaje.Estado == "CANCELADO")
            {
                return BadRequest("El viaje ya está cancelado");
            }

            var fechaHoraViaje = viaje.Fecha.Date + viaje.Hora;

            if (fechaHoraViaje <= DateTime.Now.AddMinutes(30))
            {
                return BadRequest(
                    "No se puede cancelar el viaje con menos de 30 minutos de anticipación");
            }

            viaje.Estado = "CANCELADO";
            viaje.MotivoCancelacion = dto.Motivo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Viaje cancelado con éxito"
            });
        }

        [HttpPost("viajes/solicitar")]
        public async Task<IActionResult> SolicitarViaje(SolicitarViajeDTO dto)
        {
            var viaje = new Viaje()
            {
                Origen = dto.Origen,
                Destino = dto.Destino,
                TipoSolicitud = dto.TipoSolicitud,
                Estado = "PENDIENTE",
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                EquipajeCarga = dto.EquipajeCarga,
                Monto = dto.Monto,
                IdSolicitante = dto.IdSolicitante,
                IdPasajero = dto.IdPasajero,
                IdUsuario = dto.IdUsuario,
                IdServicio = dto.IdServicio
            };

            _context.Viajes.Add(viaje);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud de viaje creada", id = viaje.Id });
        }

        [HttpPut("viajes/{id:int}/responder-contraoferta")]
        public async Task<IActionResult> ResponderContraOferta(int id, [FromBody] ResponderContraOfertaDTO dto)
        {
            var viaje = await _context.Viajes.FindAsync(id);

            if (viaje == null)
            {
                return NotFound("El viaje no existe");
            }

            if (dto.Aceptar)
            {
                viaje.Estado = "ACEPTADO";
            }
            else
            {
                viaje.Estado = "CANCELADO";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = dto.Aceptar
                    ? "Contraoferta aceptada"
                    : "Contraoferta rechazada"
            });
        }


        [HttpGet("{usuarioId:int}/viajes")]
        public async Task<ActionResult<List<ListadoViajeDTO>>> ViajesSolicitados(int usuarioId, string? estado = "PENDIENTE")
        {
            var query = _context.Viajes
                .Where(v => v.IdUsuario == usuarioId);

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(v => v.Estado == estado);
            }

            var viajes = await query
                .Select(v => new ListadoViajeDTO
                {
                    Id = v.Id,
                    Origen = v.Origen,
                    Destino = v.Destino,
                    TipoSolicitud = v.TipoSolicitud,
                    Estado = v.Estado,
                    EstadoPago = v.EstadoPago,
                    Fecha = v.Fecha,
                    Hora = v.Hora,
                    Monto = v.Monto,
                    MontoEstimado = v.MontoEstimado,
                    MetodoDePago = v.MetodoDePago,
                    EquipajeCarga = v.EquipajeCarga,
                    FechaHoraReserva = v.FechaHoraReserva
                })
                .ToListAsync();

            return Ok(viajes);
        }

        //viaje detallado de un usuario en particular, para mostrarlo en la pantalla de detalle del viaje
        [HttpGet("{usuarioId:int}/viajes/{viajeId:int}")] //api/pasajero/{usuarioId}/viajes/{viajeId}
        public async Task<ActionResult<ViajeDetalladoDTO>> GetViajeDetalle(int usuarioId, int viajeId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "Usuario no encontrado"
                });
            }

            var esAdmin = usuario.Rol == "Admin";

            var viaje = await _context.Viajes
                .Where(v => v.Id == viajeId)
                .Where(v =>
                    esAdmin ||
                    v.IdPasajero == usuarioId ||
                    v.IdUsuario == usuarioId ||
                    (v.IdChofer != 0 && _context.Choferes.Any(c => c.Id == v.IdChofer && c.IdUsuario == usuarioId)) ||
                    (v.IdChofer != 0 && _context.Choferes.Any(c => c.Id == v.IdChofer && c.IdPrestador == usuarioId))
                )
                .Select(v => new ViajeDetalladoDTO
                {
                    Id = v.Id,

                    Origen = v.Origen,
                    Destino = v.Destino,
                    TipoSolicitud = v.TipoSolicitud,

                    Estado = v.Estado,
                    EstadoPago = v.EstadoPago,

                    Fecha = v.Fecha,
                    Hora = v.Hora,
                    FechaHoraReserva = v.FechaHoraReserva,

                    Monto = v.Monto,
                    MontoEstimado = v.MontoEstimado,
                    MetodoDePago = v.MetodoDePago,

                    EquipajeCarga = v.EquipajeCarga,
                    DetalleCarga = v.DetalleCarga,
                    MotivoCancelacion = v.MotivoCancelacion,

                    IdPasajero = v.IdPasajero,
                    NombrePasajero = v.Pasajero.Nombre,

                    IdChofer = v.IdChofer,
                    NombreChofer = v.Chofer.Nombre,

                    IdVehiculo = v.IdVehiculo,
                    Patente = v.Vehiculo.Patente,
                    Marca = v.Vehiculo.Marca
                })
                .FirstOrDefaultAsync();

            if (viaje == null)
            {
                return NotFound(new
                {
                    mensaje = "Viaje no encontrado"
                });
            }

            return Ok(viaje);
        }


    }
}
