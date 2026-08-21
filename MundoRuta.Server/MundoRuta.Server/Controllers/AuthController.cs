using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            string estadoAsignado = "";

            if (request.Rol == "Usuario")
            {
                estadoAsignado = "APROBADO";
            }
            else
            {
                estadoAsignado = "PENDIENTE";
            }
            var nuevoUsuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                Password = request.Password,
                Rol = request.Rol,
                Estado = estadoAsignado
            };
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado con éxito" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas"); // Error 401
            }

            if (usuario.Estado == "PENDIENTE")
            {
                return StatusCode(403, "Cuenta pendiente de aprobación");
            }


            return Ok(new
            {
                mensaje = "Login exitoso",
                id = usuario.Id, 
                nombre = usuario.Nombre,
                rol = usuario.Rol
            });
        }

    }

}
