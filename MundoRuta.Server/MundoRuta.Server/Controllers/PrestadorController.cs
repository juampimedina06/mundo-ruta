using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Server.Hubs;
using MundoRuta.Shared;
using MundoRuta.Shared.DTO;
using MundoRuta.Shared.DTO.Realtime;

namespace MundoRuta.Server.Controllers;

[ApiController]
[Route("api/prestador")]
public class PrestadorController : ControllerBase
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IViajeRepositorio _viajeRepositorio;
    private readonly IChoferRepositorio _choferRepositorio;
    private readonly IVehiculoRepositorio _vehiculoRepositorio;
    private readonly IOfertaViajeRepositorio _ofertaViajeRepositorio;
    private readonly MundoRuta.BD.Datos.AppDbContext _context;
    private readonly IHubContext<ViajeHub> _hubContext;

    public PrestadorController(
        IUsuarioRepositorio usuarioRepositorio,
        IViajeRepositorio viajeRepositorio,
        IChoferRepositorio choferRepositorio,
        IVehiculoRepositorio vehiculoRepositorio,
        IOfertaViajeRepositorio ofertaViajeRepositorio,
        MundoRuta.BD.Datos.AppDbContext context,
        IHubContext<ViajeHub> hubContext)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _viajeRepositorio = viajeRepositorio;
        _choferRepositorio = choferRepositorio;
        _vehiculoRepositorio = vehiculoRepositorio;
        _ofertaViajeRepositorio = ofertaViajeRepositorio;
        _context = context;
        _hubContext = hubContext;
    }

    // GET: lista de solicitudes pendientes
    [HttpGet("{prestadorId:int}/solicitudes")]
    public async Task<ActionResult> GetSolicitudes(int prestadorId)
    {
        var solicitudes = await _viajeRepositorio.SolicitudesPendientes(prestadorId);
        return Ok(solicitudes);
    }

    // POST: El prestador agrega un nuevo chofer a la empresa
    [HttpPost("choferes")] // api/prestador/choferes 
    public async Task<ActionResult<Chofer>> AltaChofer(ChoferAltaDTO dto)
    {
        var prestador = await _usuarioRepositorio.ObtenerPorId(dto.IdPrestador);
        if (prestador == null || prestador.Rol != "Prestador")
        {
            return NotFound("El prestador indicado no existe.");
        }

        var existeEmail = await _usuarioRepositorio.EmailExiste(dto.Email, 0);
        if (existeEmail)
        {
            return Conflict("El email ya está registrado.");
        }

        // Le pasamos el DTO directamente al repositorio
        var choferCreado = await _choferRepositorio.CrearChoferConUsuario(dto);

        return Ok(choferCreado);
    }

    // GET: Lista los choferes de un prestador
    [HttpGet("{prestadorId:int}/choferes")] // api/prestador/{prestadorId}/choferes
    public async Task<ActionResult<List<ChoferDTO>>> GetChoferes(int prestadorId)
    {
        var listaChoferes = await _choferRepositorio.ObtenerPorPrestador(prestadorId);

        var choferesDto = listaChoferes.Select(c => new ChoferDTO
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Licencia = c.Licencia,
            Estado = c.Estado
        }).ToList();

        return Ok(choferesDto);
    }

    // GET: Información del prestador y vehículos
    [HttpGet("{id:int}/perfil-publico")] // api/prestador/{id}/perfil-publico
    public async Task<ActionResult<PrestadorPerfilPublicoDTO>> GetPerfilPublico(int id)
    {
        var prestador = await _usuarioRepositorio.ObtenerPorId(id);

        if (prestador == null || prestador.Rol != "Prestador")
        {
            return NotFound("No se encontró el prestador");
        }

        var listaVehiculos = await _vehiculoRepositorio.ObtenerPorPrestador(id);

        var vehiculosDto = listaVehiculos.Select(v => new VehiculoDTO
        {
            Id = v.Id,
            Patente = v.Patente,
            Marca = v.Marca,
            Licencia = v.Licencia,
            Estado = v.Estado
        }).ToList();

        var perfilPublico = new PrestadorPerfilPublicoDTO
        {
            Id = prestador.Id,
            Nombre = prestador.Nombre,
            Apellido = prestador.Apellido,
            Email = prestador.Email,
            Telefono = prestador.Telefono,
            FechaRegistro = prestador.FechaRegistro,
            Estado = prestador.Estado,
            Rol = prestador.Rol,
            RazonSocial = prestador.RazonSocial,
            Cuit = prestador.Cuit,
            Vehiculos = vehiculosDto
        };

        return Ok(perfilPublico);
    }

    // PUT: Responder a solicitud de viaje (crear oferta en subasta)
    [HttpPut("viajes/{id:int}/responder")]
    public async Task<IActionResult> ResponderSolicitud(int id, ResponderSolicitudDTO dto, [FromQuery] int idPrestador = 0)
    {
        var viaje = await _viajeRepositorio.ObtenerPorId(id);
        if (viaje == null)
        {
            return NotFound("No encontrado");
        }

        if (viaje.Estado != EstadosViaje.Buscando)
        {
            return BadRequest("El viaje no se encuentra en estado BUSCANDO para recibir ofertas");
        }

        int ofertasActivas = _context.OfertasViajes.Count(o => o.IdViaje == id && o.EstadoOferta == "ACTIVA");
        if (ofertasActivas >= 5)
        {
            return BadRequest("Ya se alcanzó el límite de 5 ofertas para este viaje");
        }

        if (dto.Accion == "ACEPTAR" || dto.Accion == "ACEPTAR_OFERTA" || dto.Accion == "OFERTAR" || dto.Accion == "POSTULAR")
        {
            var prestadorId = idPrestador;
            if (prestadorId <= 0 && viaje.IdUsuario > 0) prestadorId = viaje.IdUsuario;
            var oferta = new OfertaViaje
            {
                IdViaje = id,
                IdPrestador = prestadorId,
                IdChoferPropuesto = dto.IdChofer > 0 ? dto.IdChofer : null,
                MontoOfertado = dto.MontoEstimado > 0 ? dto.MontoEstimado : viaje.Monto,
                EsContraoferta = false,
                EstadoOferta = "ACTIVA",
                FechaOferta = DateTime.Now
            };
            await _context.OfertasViajes.AddAsync(oferta);
            await _context.SaveChangesAsync();

            var prestador = await _usuarioRepositorio.ObtenerPorId(prestadorId);
            var ofertaDto = new NuevaOfertaDTO
            {
                IdOferta = oferta.Id,
                IdViaje = oferta.IdViaje,
                IdPrestador = oferta.IdPrestador,
                NombrePrestador = prestador != null ? (prestador.RazonSocial ?? (prestador.Nombre + " " + prestador.Apellido)) : string.Empty,
                MontoOfertado = oferta.MontoOfertado,
                EsContraoferta = oferta.EsContraoferta,
                EstadoOferta = oferta.EstadoOferta,
                FechaOferta = oferta.FechaOferta,
                IdChoferPropuesto = oferta.IdChoferPropuesto
            };

            await _hubContext.Clients.Group($"viaje_{id}").SendAsync("NuevaOfertaRecibida", ofertaDto);
            if (viaje.IdPasajero > 0)
            {
                await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("NuevaOfertaRecibida", ofertaDto);
            }

            return Ok(new { mensaje = "Oferta registrada", idOferta = oferta.Id });
        }
        else if (dto.Accion == "RECHAZAR")
        {
            viaje.Estado = EstadosViaje.Rechazado;
            await _viajeRepositorio.Update(viaje);
            return Ok(viaje);
        }
        else
        {
            return BadRequest("Acción inválida. Debe ser 'ACEPTAR' o 'RECHAZAR'.");
        }
    }

    // PUT: Enviar contraoferta al pasajero (crear oferta en subasta)
    [HttpPut("viajes/{id:int}/contraofertar")]
    public async Task<IActionResult> ContraOfertar(int id, ContraOfertaDTO dto, [FromQuery] int idPrestador = 0)
    {
        var viaje = await _viajeRepositorio.ObtenerPorId(id);
        if (viaje == null)
        {
            return NotFound("No encontrado");
        }

        if (viaje.Estado != EstadosViaje.Buscando && viaje.Estado != EstadosViaje.Contraofertado)
        {
            return BadRequest("El viaje no permite contraofertas en este estado");
        }

        int ofertasActivas2 = _context.OfertasViajes.Count(o => o.IdViaje == id && o.EstadoOferta == "ACTIVA");
        if (ofertasActivas2 >= 5)
        {
            return BadRequest("Ya se alcanzó el límite de 5 ofertas para este viaje");
        }

        var prestadorId2 = idPrestador;
        if (prestadorId2 <= 0 && viaje.IdUsuario > 0) prestadorId2 = viaje.IdUsuario;
        var oferta2 = new OfertaViaje
        {
            IdViaje = id,
            IdPrestador = prestadorId2,
            IdChoferPropuesto = dto.IdChofer > 0 ? dto.IdChofer : null,
            MontoOfertado = dto.NuevoMonto,
            EsContraoferta = true,
            EstadoOferta = "ACTIVA",
            FechaOferta = DateTime.Now
        };
        await _context.OfertasViajes.AddAsync(oferta2);
        viaje.Estado = EstadosViaje.Contraofertado;
        await _viajeRepositorio.Update(viaje);
        await _context.SaveChangesAsync();

        var prestador2 = await _usuarioRepositorio.ObtenerPorId(prestadorId2);
        var ofertaDto2 = new NuevaOfertaDTO
        {
            IdOferta = oferta2.Id,
            IdViaje = oferta2.IdViaje,
            IdPrestador = oferta2.IdPrestador,
            NombrePrestador = prestador2 != null ? (prestador2.RazonSocial ?? (prestador2.Nombre + " " + prestador2.Apellido)) : string.Empty,
            MontoOfertado = oferta2.MontoOfertado,
            EsContraoferta = oferta2.EsContraoferta,
            EstadoOferta = oferta2.EstadoOferta,
            FechaOferta = oferta2.FechaOferta,
            IdChoferPropuesto = oferta2.IdChoferPropuesto
        };

        await _hubContext.Clients.Group($"viaje_{id}").SendAsync("NuevaOfertaRecibida", ofertaDto2);
        if (viaje.IdPasajero > 0)
        {
            await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("NuevaOfertaRecibida", ofertaDto2);
        }

        return Ok(new { mensaje = "Contraoferta enviada al pasajero", idOferta = oferta2.Id });
    }

    // POST: Registrar un nuevo vehículo
    [HttpPost("vehiculo")] // api/prestador/vehiculo
    public async Task<ActionResult<Vehiculo>> Post(RegistroVehiculoDTO dto)
    {
        var entidad = new Vehiculo
        {
            Patente = dto.Patente,
            Marca = dto.Marca,
            Licencia = dto.Licencia,
            Estado = dto.Estado,
            FechaFabricación = dto.FechaFabricación,
            NumeroConductor = dto.NumeroConductor,
            CaracteristicasConductor = dto.CaracteristicasConductor,
            TipoVehiculo = dto.TipoVehiculo,
            CapacidadCarga = dto.CapacidadCarga,
            MarcaModelo = dto.MarcaModelo,
            IdUsuario = dto.IdUsuario,
            IdChofer = dto.IdChofer > 0 ? dto.IdChofer : null
        };

        var vehiculoCreado = await _vehiculoRepositorio.Insert(entidad);
        return Ok(vehiculoCreado);
    }

    // GET: Listado de vehículos de un prestador
    [HttpGet("{id:int}/vehiculos")]
    public async Task<ActionResult<List<VehiculoDTO>>> GetVehiculos(int id)
    {
        var listaVehiculos = await _vehiculoRepositorio.ObtenerPorPrestador(id);

        if (listaVehiculos == null || !listaVehiculos.Any())
        {
            return NotFound("No se encontraron vehículos");
        }

        var vehiculosDto = listaVehiculos.Select(v => new VehiculoDTO
        {
            Id = v.Id,
            Patente = v.Patente,
            Marca = v.Marca,
            Licencia = v.Licencia,
            Estado = v.Estado,
            CapacidadCarga = v.CapacidadCarga,
            TipoVehiculo = v.TipoVehiculo,
            IdUsuario = v.IdUsuario,
            IdChofer = v.IdChofer
        }).ToList();

        return Ok(vehiculosDto);
    }

    // PUT: Finalizar un viaje
    [HttpPut("viajes/{id:int}/finalizar")]
    public async Task<IActionResult> FinalizarServicio(int id)
    {
        var viaje = await _viajeRepositorio.ObtenerPorId(id);

        if (viaje == null)
        {
            return NotFound("No encontrado");
        }

        if (viaje.Estado != EstadosViaje.EnTransito) // Verifica que el viaje esté en tránsito antes de finalizarlo.
        {
            return BadRequest("El viaje no está en curso, no se puede finalizar");
        }

        await _viajeRepositorio.FinalizarViaje(viaje);

        var estadoDto = new EstadoViajeActualizadoDTO
        {
            IdViaje = viaje.Id,
            Estado = EstadosViaje.Finalizado,
            Mensaje = "Viaje finalizado correctamente"
        };
        await _hubContext.Clients.Group($"viaje_{viaje.Id}").SendAsync("EstadoViajeActualizado", estadoDto);
        if (viaje.IdPasajero > 0)
        {
            await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("EstadoViajeActualizado", estadoDto);
        }
        if (viaje.IdChofer > 0)
        {
            await _hubContext.Clients.Group($"chofer_{viaje.IdChofer}").SendAsync("EstadoViajeActualizado", estadoDto);
        }

        return Ok(new { mensaje = "Viaje finalizado correctamente" });
    }

    // PUT: Registrar pago de un viaje
    [HttpPut("viajes/{id:int}/registrar-pago")]
    public async Task<ActionResult> RegistrarPago(int id, ConfirmacionPagoDTO dto)
    {
        var viaje = await _viajeRepositorio.ObtenerPorId(id);

        if (viaje == null)
        {
            return NotFound("Viaje no encontrado");
        }

        viaje.EstadoPago = "PAGADO";
        viaje.Monto = dto.montoCobrado;
        viaje.MetodoDePago = dto.metodoPago;

        await _viajeRepositorio.Update(viaje);

        var pagoDto = new EstadoViajeActualizadoDTO
        {
            IdViaje = viaje.Id,
            Estado = viaje.Estado,
            Mensaje = "Pago registrado"
        };
        await _hubContext.Clients.Group($"viaje_{viaje.Id}").SendAsync("EstadoViajeActualizado", pagoDto);
        if (viaje.IdPasajero > 0)
        {
            await _hubContext.Clients.Group($"pasajero_{viaje.IdPasajero}").SendAsync("EstadoViajeActualizado", pagoDto);
        }

        return Ok(new { mensaje = "Pago realizado con exito", viaje });
    }

    // PUT: Configurar tarifa por km del prestador (opción A)
    [HttpPut("{id:int}/tarifa-km")]
    public async Task<IActionResult> ConfigurarTarifaKm(int id, TarifaKmDTO dto)
    {
        var prestador = await _usuarioRepositorio.ObtenerPorId(id);
        if (prestador == null || prestador.Rol != "Prestador")
        {
            return NotFound("Prestador no encontrado");
        }

        prestador.TarifaBaseKm = dto.TarifaBaseKm;
        await _usuarioRepositorio.Update(prestador);
        return Ok(new { mensaje = "Tarifa por km actualizada", tarifaBaseKm = prestador.TarifaBaseKm });
    }
}