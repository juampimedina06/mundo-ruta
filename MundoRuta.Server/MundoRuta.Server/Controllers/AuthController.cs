using Microsoft.AspNetCore.Mvc;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public AuthController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        [HttpPost("register")] // api/auth/register
        public async Task<ActionResult<int>> Register([FromBody] RegisterRequestDto registerDTO)
        {
            var existe = await _usuarioRepositorio.EmailExiste(registerDTO.Email);

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
                FechaRegistro = DateTime.UtcNow,
                Rol = registerDTO.Rol,
                Estado = estadoAsignado,
                Cuit = esPrestador ? registerDTO.Cuit : "",
                RazonSocial = esPrestador ? registerDTO.RazonSocial : ""
            };

            var creado = await _usuarioRepositorio.Insert(nuevoUsuario);

            return Ok(creado.Id);
        }
        [HttpPost("login")] // api/auth/login
        public async Task<ActionResult> Login(LoginRequestDto loginDTO)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorEmail(loginDTO.Email);

            if (usuario == null || usuario.Password != loginDTO.Password)
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