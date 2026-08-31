using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/admin")] 
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("prestadores-pendientes")] //api/admin/prestadores-pendientes
        public async Task<ActionResult<List<PrestadorListadoDTO>>> ListaPrestadoresPendientes()
        {
            var listaPrestadoresPendientes = await _context.Usuarios
        .Where(u => u.Rol == "Prestador" && u.Estado == "PENDIENTE")
        .Select(u => new PrestadorListadoDTO
        {
            Email = u.Email,
            Telefono = u.Telefono,
            Estado = u.Estado,
            Rol = u.Rol
        })
        .ToListAsync();

            if(listaPrestadoresPendientes == null || !listaPrestadoresPendientes.Any())
            {
                return NotFound(new { mensaje = "No hay prestadores pendientes" });
            }

            return Ok(listaPrestadoresPendientes);
        }

        
        [HttpPut("aprobar-prestador/{id:int}")]
        public async Task<IActionResult> AprobarPrestador(int id)
        {
            var prestador = await _context.Usuarios
              .FirstOrDefaultAsync(u => u.Id == id && u.Rol == "Prestador");

            if (prestador == null)
            {
                return NotFound(new { mensaje = "Prestador no encontrado" });
            }

            prestador.Estado = "APROBADO";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Prestador aprobado correctamente" });
        }


        [HttpGet("dashboard")] //api/admin/dashboard
        public async Task<IActionResult> GetDashboard()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync(u => u.Rol == "Usuario");
            var totalPrestadores = await _context.Usuarios.CountAsync(u => u.Rol == "Prestador" && u.Estado == "APROBADO");
            var viajesConcretados = await _context.Viajes.CountAsync(v => v.Estado == "FINALIZADO");

            return Ok(new
            {
                totalUsuarios,
                totalPrestadores,
                viajesConcretados
            });
        }
    }
}