using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers;

[ApiController]
[Route("api/prestador")]
public class PrestadorController : ControllerBase
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IViajeRepositorio _viajeRepositorio;
    private readonly IChoferRepositorio _choferRepositorio;
    private readonly IVehiculoRepositorio _vehiculoRepositorio;

    public PrestadorController(
        IUsuarioRepositorio usuarioRepositorio,
        IViajeRepositorio viajeRepositorio,
        IChoferRepositorio choferRepositorio,
        IVehiculoRepositorio vehiculoRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _viajeRepositorio = viajeRepositorio;
        _choferRepositorio = choferRepositorio;
        _vehiculoRepositorio = vehiculoRepositorio;
    }

    // GET: lista de solicitudes pendientes
    [HttpGet("{prestadorId:int}/solicitudes")]
    public async Task<ActionResult> GetSolicitudes(int prestadorId)
    {
        var solicitudes = await _viajeRepositorio.ListadoPorPrestador(prestadorId, "PENDIENTE");
        return Ok(solicitudes);
    }

    // GET: todos los viajes de la empresa
    [HttpGet("{prestadorId:int}/viajes")] // api/prestador/{prestadorId}/viajes
    public async Task<ActionResult<List<ViajePrestadorDTO>>> GetViajes(int prestadorId, string? estado = null)
    {
        var viajes = await _viajeRepositorio.ListadoPorPrestador(prestadorId, estado);
        return Ok(viajes);
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

    // PUT: Responder a solicitud de viaje
    [HttpPut("viajes/{id:int}/responder")]
    public async Task<IActionResult> ResponderSolicitud(int id, ResponderSolicitudDTO dto)
    {
        var viaje = await _viajeRepositorio.ObtenerPorId(id);
        if (viaje == null)
        {
            return NotFound("No encontrado");
        }

        if (dto.Accion == "ACEPTAR")
        {
            viaje.Estado = "Aceptado";
            viaje.IdChofer = dto.IdChofer;
            viaje.IdVehiculo = dto.IdVehiculo;
        }
        else if (dto.Accion == "RECHAZAR")
        {
            viaje.Estado = "Rechazado";
        }
        else
        {
            return BadRequest("Acción inválida. Debe ser 'ACEPTAR' o 'RECHAZAR'.");
        }

        await _viajeRepositorio.Update(viaje);
        return Ok(viaje);
    }

    // PUT: Enviar contraoferta al pasajero
    [HttpPut("viajes/{id:int}/contraofertar")]
    public async Task<IActionResult> ContraOfertar(int id, ContraOfertaDTO dto)
    {
        var viaje = await _viajeRepositorio.ObtenerPorId(id);
        if (viaje == null)
        {
            return NotFound("No encontrado");
        }

        viaje.Estado = "CONTRAOFERTADO";
        viaje.Monto = dto.NuevoMonto;
        viaje.IdChofer = dto.IdChofer;
        viaje.IdVehiculo = dto.IdVehiculo;

        await _viajeRepositorio.Update(viaje);
        return Ok(new { mensaje = "Contraoferta enviada al pasajero" });
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
        return Ok(listaVehiculos);
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

        if (viaje.Estado != "EN_CURSO")
        {
            return BadRequest("El viaje no está en curso, no se puede finalizar");
        }

        await _viajeRepositorio.FinalizarViaje(viaje);

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
        return Ok(new { mensaje = "Pago realizado con exito", viaje });
    }
}