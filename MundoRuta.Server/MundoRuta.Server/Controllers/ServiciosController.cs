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

    // GET 
    // Muestra el listado de todos los Prestadores disponibles para contratar.
    [HttpGet("fletes")]//api/servicios/fletes
    public async Task<ActionResult<List<PrestadorListadoDTO>>> GetFletes()
    {
        var prestadores = await context.Usuarios
            .Where(p => p.Rol == "Prestador" && p.Estado == "APROBADO")
            .Select(p => new PrestadorListadoDTO
            {
                Id = p.Id,
                Email = p.Email,
                Estado = p.Estado,
                Rol = p.Rol,

            })
            .ToListAsync();

        return Ok(prestadores);
    }
}
