using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly AppDbContext context;

        public UsuarioRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            return await context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObtenerPorEmail(string email)
        {
            return await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExiste(string email, int idExcluir = 0)
        {
            return await context.Usuarios.AnyAsync(u => u.Email == email && u.Id != idExcluir);
        }

        public async Task<Usuario?> ValidarCredenciales(string email, string password)
        {
            return await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<List<PrestadorListadoDTO>> ObtenerPrestadoresPorEstado(string estado)
        {
            return await context.Usuarios
                .Where(u => u.Rol == "Prestador" && u.Estado == estado)
                .Select(u => new PrestadorListadoDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Telefono = u.Telefono,
                    Estado = u.Estado,
                    Rol = u.Rol
                })
                .ToListAsync();
        }

        public async Task<bool> CambiarEstado(int id, string nuevoEstado)
        {
            var usuario = await context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            usuario.Estado = nuevoEstado;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ContarPorRolYEstado(string rol, string? estado = null)
        {
            var query = context.Usuarios.Where(u => u.Rol == rol);
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(u => u.Estado == estado);
            }
            return await query.CountAsync();
        }
    }
}