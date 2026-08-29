using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers;

[ApiController]
[Route("api/servicios")]
public class ServiciosController : ControllerBase
{
    private readonly AppDbContext context;

    public ServiciosController(AppDbContext context)
    {
        this.context = context;
    }

    // GET /api/servicios/fletes
    // Muestra el listado de todos los Prestadores disponibles para contratar.
    [HttpGet("fletes")]
    public async Task<IActionResult> GetFletes()
    {
        var prestadores = await context.Prestadores
            .Where(p => p.Estado == "APROBADO")
            .Select(p => new PrestadorListadoDTO
            {
                Id = p.Id,
                RazonSocial = p.RazonSocial,
                Telefono = p.Telefono,
                Email = p.Email,
                Ciudad = p.Ciudad
            })
            .ToListAsync();

        return Ok(prestadores);
    }
}
