using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public class ViajeRepositorio : Repositorio<Viaje>, IViajeRepositorio
    {
        private readonly AppDbContext context;

        public ViajeRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Viaje?> ObtenerPorId(int id)
        {
            return await context.Viajes.FindAsync(id);
        }

        public async Task InsertarConServicio(Viaje viaje)
        {
            if (viaje.IdServicio == 0)
            {
                var servicioDefault = await context.Servicios.FirstOrDefaultAsync();
                if (servicioDefault == null)
                {
                    servicioDefault = new Servicio { Nombre = "Flete", Descripcion = "Flete General", Tipo = "F", Estado = "ACTIVO" };
                    context.Servicios.Add(servicioDefault);
                    await context.SaveChangesAsync();
                }
                viaje.IdServicio = servicioDefault.Id;
            }

            context.Viajes.Add(viaje);
            await context.SaveChangesAsync();
        }

        public async Task<List<Viaje>> ObtenerPorUsuario(int usuarioId, string? estado)
        {
            var query = context.Viajes.Where(v => v.IdUsuario == usuarioId || v.IdPasajero == usuarioId);
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(v => v.Estado == estado);
            }
            return await query.ToListAsync();
        }

        public async Task<Viaje?> ObtenerDetalle(int viajeId, int usuarioId, bool esPrestador)
        {
            return await context.Viajes
                .Include(v => v.Pasajero)
                .Include(v => v.Chofer)
                .Include(v => v.Vehiculo)
                .FirstOrDefaultAsync(v => v.Id == viajeId);
        }

        public async Task<List<Viaje>> SolicitudesPendientes(int prestadorId) // Obtiene las solicitudes de viaje que todavía están buscando un prestador.
        {
            return await context.Viajes  // Filtra los viajes del prestador indicado, y que estén en estado "buscando".
                .Where(s => s.IdUsuario == prestadorId && s.Estado == EstadosViaje.Buscando)
                .ToListAsync();
        }

        public async Task<bool> ResponderSolicitud(int viajeId, ResponderSolicitudDTO dto)
        {
            var viaje = await context.Viajes.FindAsync(viajeId);
            if (viaje == null) return false;

            if (dto.Accion == "ACEPTAR")
            {
                viaje.Estado = EstadosViaje.Asignado;
                viaje.IdChofer = dto.IdChofer;
                viaje.IdVehiculo = dto.IdVehiculo;
            }
            else if (dto.Accion == "RECHAZAR")
            {
                viaje.Estado = "Rechazado";
            }
            else
            {
                return false;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ContraOfertar(int viajeId, ContraOfertaDTO dto)
        {
            var viaje = await context.Viajes.FindAsync(viajeId);
            if (viaje == null) return false;

            viaje.Estado = "CONTRAOFERTADO";
            viaje.Monto = dto.NuevoMonto;
            viaje.IdChofer = dto.IdChofer;
            viaje.IdVehiculo = dto.IdVehiculo;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task IniciarViaje(Viaje viaje)
        {
            context.Viajes.Update(viaje);
            await context.SaveChangesAsync();
        }

        public async Task FinalizarViaje(Viaje viaje)
        {
            viaje.Estado = EstadosViaje.Finalizado;
            viaje.Fecha = DateTime.UtcNow;
            await context.SaveChangesAsync();

            if (viaje.IdChofer > 0)
            {
                var chofer = await context.Choferes.FindAsync(viaje.IdChofer);
                if (chofer != null)
                {
                    chofer.Estado = "DISPONIBLE";
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> RegistrarPago(int viajeId, ConfirmacionPagoDTO dto)
        {
            var viaje = await context.Viajes.FindAsync(viajeId);
            if (viaje == null) return false;

            viaje.EstadoPago = "PAGADO";
            viaje.Monto = dto.montoCobrado;
            viaje.MetodoDePago = dto.metodoPago;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ContarViajesFinalizados()
        {
            return await context.Viajes.CountAsync(v => v.Estado == EstadosViaje.Finalizado);
        }
    }
}