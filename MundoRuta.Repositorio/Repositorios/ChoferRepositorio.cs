using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public class ChoferRepositorio : Repositorio<Chofer>, IChoferRepositorio
    {
        private readonly AppDbContext context;

        public ChoferRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Chofer> CrearChoferConUsuario(ChoferAltaDTO dto)
        {
            var usuarioChofer = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Password = dto.Password,
                Rol = "Chofer",
                Estado = "APROBADO",
                Cuit = dto.Cuit,
                RazonSocial = "",
                Telefono = dto.Telefono.ToString()
            };

            var chofer = new Chofer
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Licencia = dto.Licencia,
                Estado = dto.Estado,
                IdPrestador = dto.IdPrestador
            };

            using var transaction = await context.Database.BeginTransactionAsync();
            context.Usuarios.Add(usuarioChofer);
            await context.SaveChangesAsync();

            chofer.IdUsuario = usuarioChofer.Id;
            context.Choferes.Add(chofer);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
            return chofer;
        }

        public async Task<List<ChoferDTO>> ObtenerPorPrestador(int prestadorId)
        {
            return await context.Choferes
                .Where(c => c.IdPrestador == prestadorId)
                .Select(c => new ChoferDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Licencia = c.Licencia,
                    Estado = c.Estado
                })
                .ToListAsync();
        }

        public async Task CambiarEstado(int choferId, string nuevoEstado)
        {
            var chofer = await context.Choferes.FindAsync(choferId);
            if (chofer != null)
            {
                chofer.Estado = nuevoEstado;
                await context.SaveChangesAsync();
            }
        }
    }
}