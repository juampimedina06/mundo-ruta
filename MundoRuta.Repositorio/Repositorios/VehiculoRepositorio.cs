using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public class VehiculoRepositorio : Repositorio<Vehiculo>, IVehiculoRepositorio
    {
        private readonly AppDbContext context;

        public VehiculoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<VehiculoDTO>> ObtenerPorPrestador(int prestadorId)
        {
            return await context.Vehiculos
                .Where(v => context.Choferes.Any(c => c.Id == v.IdChofer && c.IdPrestador == prestadorId))
                .Select(v => new VehiculoDTO
                {
                    Id = v.Id,
                    Patente = v.Patente,
                    Marca = v.Marca,
                    Licencia = v.Licencia,
                    Estado = v.Estado,
                    CapacidadCarga = v.CapacidadCarga,
                    TipoVehiculo = v.TipoVehiculo,
                    IdUsuario = v.IdUsuario,
                    IdChofer = v.IdChofer
                })
                .ToListAsync();
        }

        public async Task<Vehiculo> RegistrarVehiculo(RegistroVehiculoDTO dto)
        {
            var entidad = new Vehiculo
            {
                Patente = dto.Patente,
                Marca = dto.Marca,
                Licencia = dto.Licencia,
                Estado = dto.Estado,
                FechaFabricación = dto.FechaFabricación,
                NumeroConductor = dto.NumeroConductor,
                CaracteristicasConductor = dto.CaracteristicasConductor,
                TipoVehiculo = dto.TipoVehiculo,
                CapacidadCarga = dto.CapacidadCarga,
                MarcaModelo = dto.MarcaModelo,
                IdUsuario = dto.IdUsuario,
                IdChofer = dto.IdChofer > 0 ? dto.IdChofer : null
            };

            context.Vehiculos.Add(entidad);
            await context.SaveChangesAsync();
            return entidad;
        }
    }
}