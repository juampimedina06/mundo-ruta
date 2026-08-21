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
    public IActionResult GetSolicitudes(int prestadorId)
    {
        var solicitudes = context.Viajes.Where(s => s.IdPrestador == prestadorId && s.Estado == "Pendiente").ToList();

        return Ok(solicitudes);
    }

    // POST /api/prestador/choferes
    // El prestador agrega un nuevo chofer a la empresa.
    [HttpPost("choferes")]
    public async Task<IActionResult> AltaChofer([FromBody] ChoferAltaDTO dto)
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
    public async Task<IActionResult> GetChoferes(int prestadorId)
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
    }

    [HttpPut("viajes/{id}/responder")]
    public async Task<IActionResult> ResponderSolicitud(int id, [FromBody] ResponderSolicitudDTO dto)
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
    public async Task<IActionResult> ContraOfertar(int id, [FromBody] ContraOfertaDTO dto)
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


}
