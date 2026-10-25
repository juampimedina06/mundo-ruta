using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Server.Hubs;
using MundoRuta.Shared;
using MundoRuta.Shared.DTO;
using MundoRuta.Shared.DTO.Realtime;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/pasajero")]
    public class PasajeroController : ControllerBase
    {
        private readonly IViajeRepositorio repositorio;
        private readonly AppDbContext _context;
        private readonly IHubContext<ViajeHub> _hubContext;

        public PasajeroController(IViajeRepositorio repositorio, AppDbContext context, IHubContext<ViajeHub> hubContext)
        {
            this.repositorio = repositorio;
            _context = context;
            _hubContext = hubContext;
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
            decimal porcentajeRecargo = 15m;
            var configRecargo = await _context.ConfiguracionesSistema.FirstOrDefaultAsync(c => c.Clave == "RECARGO_ACOMPANANTE_PORCENTAJE");
            if (configRecargo != null && decimal.TryParse(configRecargo.Valor.Replace(".", ","), out decimal pCfg))
            {
                porcentajeRecargo = pCfg;
            }
            else if (configRecargo != null && decimal.TryParse(configRecargo.Valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal pCfgInv))
            {
                porcentajeRecargo = pCfgInv;
            }

            decimal montoCalculado = dto.Monto;
            if (dto.LlevaAcompanante)
            {
                montoCalculado = dto.Monto * (1 + (porcentajeRecargo / 100m));
            }

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
                EsCubierto = dto.EsCubierto,
                UsaBaulera = dto.UsaBaulera,
                LlevaAcompanante = dto.LlevaAcompanante,
                DniReceptor = dto.DniReceptor ?? string.Empty,
                DetalleCarga = dto.detalleCarga,
                Monto = montoCalculado,
                MontoEstimado = montoCalculado,
                IdSolicitante = dto.IdSolicitante,
                IdPasajero = dto.IdPasajero,
                IdUsuario = dto.IdUsuario,
                IdServicio = dto.IdServicio,
                IdChofer = dto.IdChofer,
                IdVehiculo = dto.IdVehiculo,
                FechaHoraReserva = DateTime.Now
            };

            await repositorio.InsertarConServicio(viaje); // Insertar el viaje y crear el servicio si no existe

            return Ok(new { mensaje = "Solicitud de viaje creada", id = viaje.Id, monto = montoCalculado });
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

        [HttpGet("viajes/{id:int}/ofertas")]
        public async Task<ActionResult<List<OfertaViajeDTO>>> GetOfertas(int id)
        {
            var viaje = await repositorio.ObtenerPorId(id);
            if (viaje == null)
            {
                return NotFound("EL VIAJE NO EXISTE");
            }

            var ofertas = await _context.OfertasViajes
                .Include(o => o.Prestador)
                .Where(o => o.IdViaje == id && o.EstadoOferta == "ACTIVA")
                .OrderBy(o => o.MontoOfertado)
                .ToListAsync();

            if (!ofertas.Any())
            {
                return Ok(new List<OfertaViajeDTO>());
            }

            var prestadorIds = ofertas.Select(o => o.IdPrestador).Distinct().ToList();
            var promedios = await _context.Calificaciones
                .Where(c => prestadorIds.Contains(c.IdUsuarioPrestador))
                .GroupBy(c => c.IdUsuarioPrestador)
                .Select(g => new { IdPrestador = g.Key, Promedio = g.Average(x => (double)x.Puntaje), Total = g.Count() })
                .ToListAsync();

            decimal montoMinimo = ofertas.Min(o => o.MontoOfertado);
            double promedioMaximo = promedios.Any() ? promedios.Max(p => p.Promedio) : 0;

            var resultado = ofertas.Select(o => {
                var promedio = promedios.FirstOrDefault(p => p.IdPrestador == o.IdPrestador)?.Promedio ?? 0;
                return new OfertaViajeDTO
                {
                    IdOferta = o.Id,
                    IdPrestador = o.IdPrestador,
                    NombrePrestador = o.Prestador != null ? (o.Prestador.RazonSocial ?? (o.Prestador.Nombre + " " + o.Prestador.Apellido)) : string.Empty,
                    MontoOfertado = o.MontoOfertado,
                    PromedioCalificacionPrestador = promedio,
                    EsMasBarata = o.MontoOfertado == montoMinimo,
                    EsMejorCalificado = promedioMaximo > 0 && Math.Abs(promedio - promedioMaximo) < 0.0001,
                    EsContraoferta = o.EsContraoferta,
                    EstadoOferta = o.EstadoOferta,
                    FechaOferta = o.FechaOferta
                };
            }).ToList();

            return Ok(resultado);
        }

        [HttpPut("viajes/{id:int}/seleccionar-oferta/{ofertaId:int}")]
        public async Task<IActionResult> SeleccionarOferta(int id, int ofertaId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var viaje = await _context.Viajes.FirstOrDefaultAsync(v => v.Id == id);
                if (viaje == null)
                {
                    return NotFound("EL VIAJE NO EXISTE");
                }

                if (viaje.Estado != EstadosViaje.Buscando && viaje.Estado != EstadosViaje.Contraofertado)
                {
                    return BadRequest("EL VIAJE NO SE ENCUENTRA DISPONIBLE PARA SELECCIONAR OFERTA");
                }

                var ofertaSeleccionada = await _context.OfertasViajes
                    .Include(o => o.Prestador)
                    .FirstOrDefaultAsync(o => o.Id == ofertaId && o.IdViaje == id && o.EstadoOferta == "ACTIVA");

                if (ofertaSeleccionada == null)
                {
                    return BadRequest("LA OFERTA NO ES VÁLIDA O YA FUE PROCESADA");
                }

                viaje.Estado = EstadosViaje.Asignado;
                viaje.IdUsuario = ofertaSeleccionada.IdPrestador;
                if (ofertaSeleccionada.IdChoferPropuesto.HasValue && ofertaSeleccionada.IdChoferPropuesto.Value > 0)
                {
                    viaje.IdChofer = ofertaSeleccionada.IdChoferPropuesto.Value;
                }
                viaje.Monto = ofertaSeleccionada.MontoOfertado;

                ofertaSeleccionada.EstadoOferta = "ACEPTADA";

                var otrasOfertas = await _context.OfertasViajes
                    .Where(o => o.IdViaje == id && o.EstadoOferta == "ACTIVA" && o.Id != ofertaId)
                    .ToListAsync();

                foreach (var o in otrasOfertas)
                {
                    o.EstadoOferta = "RECHAZADA";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var asignadoDto = new ViajeAsignadoDTO
                {
                    IdViaje = viaje.Id,
                    IdPrestador = ofertaSeleccionada.IdPrestador,
                    IdChofer = viaje.IdChofer > 0 ? viaje.IdChofer : ofertaSeleccionada.IdChoferPropuesto,
                    Monto = viaje.Monto,
                    Estado = viaje.Estado
                };
                await _hubContext.Clients.Group($"viaje_{viaje.Id}").SendAsync("ViajeAsignado", asignadoDto);
                await _hubContext.Clients.Group($"prestador_{ofertaSeleccionada.IdPrestador}").SendAsync("ViajeAsignado", asignadoDto);
                if (viaje.IdChofer > 0)
                {
                    await _hubContext.Clients.Group($"chofer_{viaje.IdChofer}").SendAsync("ViajeAsignado", asignadoDto);
                }
                if (viaje.IdPasajero > 0)
                {
                    await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("ViajeAsignado", asignadoDto);
                }

                return Ok(new { mensaje = "OFERTA SELECCIONADA CORRECTAMENTE", idViaje = viaje.Id, monto = viaje.Monto });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("ERROR AL SELECCIONAR LA OFERTA");
            }
        }
    }
}