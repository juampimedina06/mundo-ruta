using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe en la base de datos"
                });
            }

            if (usuario.Estado == "Inactivo")
            {
                return BadRequest(new
                {
                    mensaje = "El usuario se encuentra inactivo"
                });
            }

            var dto = new UsuarioDTO
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                FechaRegistro = usuario.FechaRegistro,
                Estado = usuario.Estado,
                Rol = usuario.Rol,
                RazonSocial = usuario.RazonSocial,
                Cuit = usuario.Cuit
            };

            return Ok(dto);
        }


        [HttpPut("{id : int}")]
        public async Task<ActionResult> UpdateUsuario(int id, UpdateUserRequestDto request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario no existe" });
            }

            var emailExiste = await _context.Usuarios
        .AnyAsync(u => u.Email == request.Email && u.Id != id);

            if (emailExiste)
            {
                return Conflict(new
                {
                    mensaje = "El email ya está registrado"
                });
            }

            usuario.Nombre = request.Nombre;
            usuario.Telefono = request.Telefono;
            usuario.Apellido = request.Apellido;
            usuario.Email = request.Email;
            usuario.Password = request.Password;
            usuario.RazonSocial = request.RazonSocial;
            usuario.Cuit = request.Cuit;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Perfil actualizado correctamente",
                usuarioActualizado = new UsuarioDTO
                {
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    Telefono = usuario.Telefono,
                    FechaRegistro = usuario.FechaRegistro,
                    Estado = usuario.Estado,
                    Rol = usuario.Rol,
                    RazonSocial = usuario.RazonSocial,
                    Cuit = usuario.Cuit
                }
            });
        }
    }
}
