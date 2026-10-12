using Microsoft.AspNetCore.Mvc;
using MundoRuta.Repositorio.Repositorios;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Server.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio repositorio;

        public UsuarioController(IUsuarioRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet("{id:int}")]
        // Obtiene un usuario por su ID
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await repositorio.ObtenerPorId(id);

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

        [HttpPut("{id:int}")]
        // Actualiza un usuario existente
        public async Task<ActionResult> UpdateUsuario(int id, UpdateUserRequestDto request)
        {
            var usuario = await repositorio.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario no existe" });
            }

            var emailExiste = await repositorio.EmailExiste(request.Email, id);

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

            await repositorio.Update(usuario);

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