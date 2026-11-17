using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared;
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

        public async Task<Chofer?> ObtenerPorUsuario(int idUsuario)
        {
            return await context.Choferes.FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);
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
        public async Task<Chofer> ObtenerPorIdAsync(int id)
        {
            // Devuelve el chofer buscando por su ID
            return await context.Choferes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ActualizarChoferAsync(int id, ChoferUpdateDto dto)
        {
            var chofer = await context.Choferes.FindAsync(id);
            if (chofer == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Telefono))
            {
                chofer.Telefono = dto.Telefono;
            }
            chofer.Licencia = dto.Licencia;
            chofer.Estado = dto.EstadoOperativo;

            if (chofer.IdUsuario > 0)
            {
                var usuario = await context.Usuarios.FindAsync(chofer.IdUsuario);
                if (usuario != null)
                {
                    bool esInactivo = string.Equals(dto.EstadoOperativo, "INACTIVO", StringComparison.OrdinalIgnoreCase);
                    usuario.Estado = esInactivo ? "INACTIVO" : "APROBADO";
                    if (!string.IsNullOrWhiteSpace(dto.Telefono))
                    {
                        usuario.Telefono = dto.Telefono;
                    }
                }
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TieneViajesEnTransitoAsync(int id)
        {
            return await context.Viajes.AnyAsync(v => v.IdChofer == id && EstadosViaje.Activos.Contains(v.Estado));
        }

        public async Task<bool> BajaLogicaAsync(int id)
        {
            var chofer = await context.Choferes.FindAsync(id);
            if (chofer == null) return false;

            chofer.Estado = "Inactivo";

            if (chofer.IdUsuario > 0)
            {
                var usuario = await context.Usuarios.FindAsync(chofer.IdUsuario);
                if (usuario != null)
                {
                    usuario.Estado = "INACTIVO";
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}