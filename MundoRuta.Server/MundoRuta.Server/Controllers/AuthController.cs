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
            // 1. Buscamos al usuario en la base de datos (¡Esto te lo resuelvo yo!)
            // Esto va a la tabla Usuarios y busca el que coincida con el Email y la Contraseña.
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            // 2. Validar si existe (¡También te lo resuelvo!)
            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas"); // Error 401
            }

            // 3. Validar el estado (¡ESTA ES TUYA!)
            // TAREA: Si la propiedad Estado del usuario es igual a "PENDIENTE", devolvé el error.
            if (usuario.Estado == "PENDIENTE")
            {
                return StatusCode(403, "Cuenta pendiente de aprobación");
            }

            // 4. Devolver los datos (¡ESTA TAMBIÉN ES TUYA!)
            // TAREA: Enganchá las propiedades del 'usuario' para que se devuelvan en el JSON.
            return Ok(new
            {
                mensaje = "Login exitoso",
                id = usuario.Id, // <-- Mirá cómo enganché el Id
                nombre = usuario.Nombre, // <-- Mirá cómo enganché el Nombre
                rol = usuario.Rol// <-- ¡Te falta enganchar el Rol a vos!
            });
        }

    }

}
