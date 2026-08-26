using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;

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

        [HttpGet("prestadores-pendientes")]
        public async Task<IActionResult> GetPrestadoresPendientes()
        {
            var pendientes = await _context.Usuarios
                .Where(u => u.Rol == "Prestador" && u.Estado == "PENDIENTE")
                .ToListAsync(); 

            return Ok(pendientes);
        }


        [HttpPut("aprobar-prestador/{id}")]
        public async Task<IActionResult> AprobarPrestador(int id)
        {
            var prestador = await _context.Usuarios.FindAsync(id);

            if (prestador == null)
            {
                return NotFound(new { mensaje = "Prestador no encontrado" });
            }

            prestador.Estado = "APROBADO";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Prestador aprobado correctamente" });
        }


        [HttpGet("dashboard")]
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