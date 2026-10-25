using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/pasajero")]
    public class PasajeroController : ControllerBase
    {
        private readonly IViajeRepositorio repositorio;

        public PasajeroController(IViajeRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpPut("viajes/{id:int}/cancelar")]
        public async Task<IActionResult> CancelarViaje(int id, CancelarViajeDTO dto)
        {
            var viaje = await repositorio.ObtenerPorId(id);

            if (viaje == null)
            {
                return NotFound("El viaje no existe");
            }

            if (viaje.Estado == EstadosViaje.Cancelado) // Verifica si el viaje ya fue cancelado.
            {
                return BadRequest("El viaje ya está cancelado");
            }

            var fechaHoraViaje = viaje.Fecha.Date + viaje.Hora;

            if (fechaHoraViaje <= DateTime.Now.AddMinutes(30))
            {
                return BadRequest(
                    "No se puede cancelar el viaje con menos de 30 minutos de anticipación");
            }

            viaje.Estado = EstadosViaje.Cancelado; // Cambia el estado del viaje a cancelado.
            viaje.MotivoCancelacion = dto.Motivo;

            await repositorio.Update(viaje);

            return Ok(new { mensaje = "Viaje cancelado con éxito" });
        }

        [HttpPost("viajes/solicitudes")]
        public async Task<IActionResult> SolicitarViaje(SolicitarViajeDTO dto)
        {
            var viaje = new Viaje()
            {
                Origen = dto.Origen,
                Destino = dto.Destino,
                TipoSolicitud = dto.TipoSolicitud,
                Estado = EstadosViaje.Buscando, // El viaje comienza buscando un prestador.
                EstadoPago = "PENDIENTE",
                MetodoDePago = "EFECTIVO",
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                EquipajeCarga = dto.EquipajeCarga,
                DetalleCarga = dto.detalleCarga,
                Monto = dto.Monto,
                IdSolicitante = dto.IdSolicitante,
                IdPasajero = dto.IdPasajero,
                IdUsuario = dto.IdUsuario,
                IdServicio = dto.IdServicio,
                IdChofer = dto.IdChofer,
                IdVehiculo = dto.IdVehiculo
            };

            await repositorio.InsertarConServicio(viaje); // Insertar el viaje y crear el servicio si no existe

            return Ok(new { mensaje = "Solicitud de viaje creada", id = viaje.Id });
        }

        [HttpPut("viajes/{id:int}/responder-contraoferta")]
        public async Task<IActionResult> ResponderContraOferta(int id, [FromBody] ResponderContraOfertaDTO dto)
        {
            var viaje = await repositorio.ObtenerPorId(id);

            if (viaje == null)
            {
                return NotFound("El viaje no existe");
            }

            if (dto.Aceptar)
            {
                viaje.Estado = EstadosViaje.Asignado; // Si el pasajero acepta, el viaje queda asignado.
            }
            else
            {
                viaje.Estado = EstadosViaje.Cancelado;  // Si el pasajero rechaza la contraoferta, se cancela el viaje.
            }

            await repositorio.Update(viaje);

            return Ok(new
            {
                mensaje = dto.Aceptar ? "Contraoferta aceptada" : "Contraoferta rechazada"
            });
        }

        [HttpGet("{usuarioId:int}/viajes")]
        public async Task<ActionResult<List<ListadoViajeDTO>>> ViajesSolicitados(int usuarioId, string? estado = EstadosViaje.Buscando) // Obtiene los viajes del usuario, filtrando por estado si se indica.
        {
            var lista = await repositorio.ObtenerPorUsuario(usuarioId, estado);

            var listaDTO = lista.Select(v => new ListadoViajeDTO
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
            }).ToList();

            return Ok(listaDTO);
        }

        [HttpGet("{usuarioId:int}/viajes/{viajeId:int}")]
        public async Task<ActionResult<ViajeDetalladoDTO>> GetViajeDetalle(int usuarioId, int viajeId)
        {
            var v = await repositorio.ObtenerDetalle(viajeId, usuarioId, false);

            if (v == null)
            {
                return NotFound(new { mensaje = "Viaje no encontrado" });
            }

            var dto = new ViajeDetalladoDTO
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
                NombrePasajero = v.Pasajero?.Nombre,
                IdChofer = v.IdChofer,
                NombreChofer = v.Chofer?.Nombre,
                IdVehiculo = v.IdVehiculo,
                Patente = v.Vehiculo?.Patente,
                Marca = v.Vehiculo?.Marca
            };

            return Ok(dto);
        }
    }
}