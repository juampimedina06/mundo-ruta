using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("register")] //api/auth/register
        public async Task<ActionResult<int>> Register( RegisterRequestDto registerDTO)
        {
            var existe = await _context.Usuarios
        .AnyAsync(u => u.Email == registerDTO.Email);

            if (existe)
                return Conflict("El email ya está registrado.");

            var esPrestador = registerDTO.Rol == "Prestador";

            var estadoAsignado = registerDTO.Rol switch
            {
                "Usuario" => "APROBADO",
                "Prestador" => "PENDIENTE",
                _ => throw new ArgumentException("Rol inválido")
            };

            var nuevoUsuario = new Usuario
            {
                Nombre = registerDTO.Nombre,
                Apellido = registerDTO.Apellido,
                Email = registerDTO.Email,
                Telefono = registerDTO.Telefono,
                Password = registerDTO.Password,
                FechaRegistro = DateTime.Now,
                Rol = registerDTO.Rol,
                Estado = estadoAsignado,
                Cuit = esPrestador ? registerDTO.Cuit : "",
                RazonSocial = esPrestador ? registerDTO.RazonSocial : ""
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(nuevoUsuario.Id);
        }


        [HttpPost("login")] //api/auth/login
        public async Task<ActionResult> Login(LoginRequestDto loginDTO)
        {

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password);
            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas"); 
            }

            if (usuario.Password != loginDTO.Password)
            {
                return Unauthorized("Credenciales incorrectas");
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
