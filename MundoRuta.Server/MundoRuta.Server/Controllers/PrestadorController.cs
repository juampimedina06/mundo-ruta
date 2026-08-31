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
       
        var solicitudes = context.Viajes.Where(s => s.IdUsuario == prestadorId && s.Estado == "PENDIENTE").ToList();

        return Ok(solicitudes);
    }


  
    // POST /api/prestador/choferes
    // El prestador agrega un nuevo chofer a la empresa.
    [HttpPost("choferes")]
    public async Task<ActionResult> AltaChofer([FromBody] ChoferAltaDTO dto)
    {
        var prestador = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == dto.IdUsuario && u.Rol == "Prestador");
        if (prestador == null)
        {
            return NotFound("El prestador indicado no existe.");
        }

        var chofer = new Chofer
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Licencia = dto.Licencia,
            Estado = "DISPONIBLE",
            IdUsuario = dto.IdUsuario
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
            .Where(c => c.IdUsuario == prestadorId)
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
    }

    // GET 
    // Muestra la información detallada de un Prestador antes de contratarlo,
    // incluyendo la lista de vehículos de sus choferes.
    [HttpGet("{id}/perfil-publico")]//api/prestador/{id}/perfil-publico
    public async Task<ActionResult<PrestadorPerfilPublicoDTO>> GetPerfilPublico(int id)
    {

        var prestador = await context.Usuarios
               .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Prestador");

        if (prestador == null)
        {
            return NotFound("no se encontro el prestador");
        }

        var vehiculos = await context.Vehiculos
            .Where(v => context.Choferes
                .Any(c => c.Id == v.IdChofer && c.IdUsuario == id))
            .Select(v => new VehiculoDTO
            {
                Id = v.Id,
                Patente = v.Patente,
                Marca = v.Marca,
                Licencia = v.Licencia,
                Estado = v.Estado
            })
            .ToListAsync();

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
            Vehiculos = vehiculos
        };

        return Ok(perfilPublico);
    }

    [HttpPut("viajes/{id}/responder")]
    public async Task<IActionResult> ResponderSolicitud(int id, ResponderSolicitudDTO dto)
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
    public async Task<IActionResult> ContraOfertar(int id, ContraOfertaDTO dto)
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
        entidad.IdUsuario = DTO.IdUsuario;

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

    [HttpPut("viajes/{id}/finalizar")]
    public async Task<IActionResult> FinalizarServicio(int id)
    {
        var viaje = await context.Viajes.FindAsync(id);

        if (viaje == null)
        {
            return NotFound("No encontrado");
        }

        if (viaje.Estado != "EN_CURSO")
        {
            return BadRequest("El viaje no está en curso, no se puede finalizar");
        }
           
        viaje.Estado = "FINALIZADO";
        viaje.Fecha = DateTime.UtcNow; // Actualizar la fecha de finalización del viaje
        await context.SaveChangesAsync();

        //liberamos al chofer

        var chofer = await context.Choferes.FindAsync(viaje.IdChofer);
        if (chofer != null)
        {
            chofer.Estado = "DISPONIBLE";
            await context.SaveChangesAsync();
        }

        return Ok(new { mensaje = "Viaje finalizado correctamente" });


    }

    [HttpPut("viajes/{id}/registrar-pago")]
    public async Task<ActionResult> RegistrarPago (int id, ConfirmacionPagoDTO dto)
    {
        var viaje = await context.Viajes.FindAsync(id);

        if (viaje == null)
        {
            return NotFound("Viaje no encontrado");
        }

        viaje.EstadoPago = "PAGADO";
        viaje.Monto = dto.montoCobrado;
        viaje.MetodoDePago = dto.metodoPago;

        await context.SaveChangesAsync();
        return Ok(new { mensaje = "Pago realizado con exito", viaje });
        
    }




}