using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;
using System.ComponentModel;


namespace MundoRuta.Server.Controllers;

[ApiController]
[Route("api/prestador")]

public class PrestadorController : ControllerBase
{
    private readonly AppDbContext context;


    public PrestadorController(AppDbContext context)
    {
        this.context = context;
    }

    [HttpGet("{prestadorId}/solicitudes")]
    public ActionResult GetSolicitudes(int prestadorId)
    {
        var solicitudes = context.Viajes.Where(s => s.IdPrestador == prestadorId && s.Estado == "Pendiente").ToList();

        return Ok(solicitudes);
    }

    // POST /api/prestador/choferes
    // El prestador agrega un nuevo chofer a la empresa.
    [HttpPost("choferes")]
    public async Task<ActionResult> AltaChofer([FromBody] ChoferAltaDTO dto)
    {
        var prestador = await context.Prestadores.FindAsync(dto.PrestadorId);
        if (prestador == null)
        {
            return NotFound("El prestador indicado no existe.");
        }

        var chofer = new Chofer
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Licencia = dto.Licencia,
            Estado = true, // true = Disponible
            IdPrestador = dto.PrestadorId
        };

        context.Choferes.Add(chofer);
        await context.SaveChangesAsync();

        return Ok(chofer);
    }

    // GET /api/prestador/{prestadorId}/choferes
    // Lista los choferes que tiene ese prestador para que luego los pueda asignar a un flete.
    [HttpGet("{prestadorId}/choferes")]
    public async Task<ActionResult<ChoferDTO>> GetChoferes(int prestadorId)
    {
        var choferes = await context.Choferes
            .Where(c => c.IdPrestador == prestadorId)
            .Select(c => new ChoferDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Licencia = c.Licencia,
                Estado = c.Estado
            })
            .ToListAsync();

        return Ok(choferes);
    // GET /api/prestador/{id}/perfil-publico
    // Muestra la información detallada de un Prestador antes de contratarlo,
    // incluyendo la lista de vehículos de sus choferes.
    [HttpGet("{id}/perfil-publico")]
    public async Task<IActionResult> GetPerfilPublico(int id)
    {
        var prestador = await context.Prestadores.FirstOrDefaultAsync(p => p.Id == id);
        if (prestador == null)
        {
            return NotFound();
        }

        var vehiculos = await context.Vehiculos
            .Where(v => context.Choferes.Any(c => c.Id == v.IdChofer && c.IdPrestador == id))
            .Select(v => new VehiculoDTO
            {
                Id = v.Id,
                Patente = v.Patente,
                Marca = v.Marca,
                Licencia = v.Licencia,
                Estado = v.Estado
            })
            .ToListAsync();

        var perfil = new PrestadorPerfilPublicoDTO
        {
            Id = prestador.Id,
            RazonSocial = prestador.RazonSocial,
            Telefono = prestador.Telefono,
            Email = prestador.Email,
            Ciudad = prestador.Ciudad,
            Vehiculos = vehiculos
        };

        return Ok(perfil);
    }

    [HttpPut("viajes/{id}/responder")]
    public async Task<ActionResult> ResponderSolicitud(int id, [FromBody] ResponderSolicitudDTO dto)
    {
        var viaje = await context.Viajes.FindAsync(id); // Buscar el viaje por su ID
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
            return BadRequest("Acción inválida. Debe ser 'ACEPTAR' o 'RECHAZAR'."); // Validación de acción inválida
        }
        await context.SaveChangesAsync();
        return Ok(viaje);
    }

    [HttpPut("viajes/{id}/contraofertar")]
    public async Task<ActionResult> ContraOfertar(int id, [FromBody] ContraOfertaDTO dto)
    {
        var viaje = await context.Viajes.FindAsync(id);
        if (viaje == null)
        {
            return NotFound("No encontrado");
        }
        viaje.Estado = "CONTRAOFERTADO";
        viaje.Monto = dto.NuevoMonto;
        viaje.IdChofer = dto.IdChofer;
        viaje.IdVehiculo = dto.IdVehiculo;
        await context.SaveChangesAsync();
        return Ok(new { mensaje = "Contraoferta enviada al pasajero" });
    }
    [HttpPost("prestador/vehiculos")]
    public async Task<ActionResult<RegistroVehiculoDTO>> Post(RegistroVehiculoDTO DTO)
    {
        Vehiculo entidad = new Vehiculo();
        entidad.Patente = DTO.Patente;
        entidad.Marca = DTO.Marca;
        entidad.Licencia = DTO.Licencia;
        entidad.Estado = DTO.Estado;
        entidad.FechaFabricación = DTO.FechaFabricación;
        entidad.NumeroConductor = DTO.NumeroConductor;
        entidad.CaracteristicasConductor = DTO.CaracteristicasConductor;
        entidad.TipoVehiculo = DTO.TipoVehiculo;
        entidad.CapacidadCarga = DTO.CapacidadCarga;
        entidad.MarcaModelo = DTO.MarcaModelo;
        entidad.IdPrestador = DTO.IdPrestador;

        context.Vehiculos.Add(entidad);
        await context.SaveChangesAsync();


        return Ok(entidad);
    }

    [HttpGet("prestador/{id}/vehiculos")]
    public async Task<ActionResult<Vehiculo>> GetVehiculo(int id)
    {
        var vehiculo = await context.Vehiculos.Where(v => v.Id == id).ToListAsync();
        if (vehiculo == null)
        {
            return NotFound("Vehículo no encontrado");
        }
        return Ok(vehiculo);
    }

}
