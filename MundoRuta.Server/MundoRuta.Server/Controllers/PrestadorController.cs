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
