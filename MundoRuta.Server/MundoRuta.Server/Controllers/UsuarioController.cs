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

        [HttpGet("{id : int}")]
        //public async Task<ActionResult<UsuarioDTO>> GetById(int id)
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario no existe en la base de datos" });
            }
            UsuarioDTO DTO = new UsuarioDTO
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Password = usuario.Password,
                Telefono = usuario.Telefono,
                FechaRegistro = usuario.FechaRegistro,
                Estado = usuario.Estado,
                Rol = usuario.Rol,
                RazonSocial = usuario.RazonSocial,
                Cuit = usuario.Cuit
            };

            return Ok(DTO);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UpdateUserRequestDto request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario no existe" });
            }

            usuario.Nombre = request.Nombre;
            usuario.Telefono = request.Telefono;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Perfil actualizado correctamente",
                usuarioActualizado = usuario
            });
        }
    }
}
