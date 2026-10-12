//using Microsoft.EntityFrameworkCore;
//using MundoRuta.BD.Datos;
//using MundoRuta.BD.Datos.Entity;

//namespace MundoRuta.Repositorio.Repositorios
//{
//    public class NotificacionRepositorio : Repositorio<Notificacion>, INotificacionRepositorio, INotificacionRepositorio
//    {
//        private readonly AppDbContext context;

//        public NotificacionRepositorio(AppDbContext context) : base(context)
//        {
//            this.context = context;
//        }

//        public async Task<List<Notificacion>> ObtenerPorUsuario(int usuarioId)
//        {
//            return await context.Notificaciones
//                .Where(n => n.UsuarioId == usuarioId)
//                .OrderByDescending(n => n.FechaCreacion)
//                .ToListAsync();
//        }

//        public async Task<List<Notificacion>> ObtenerPorPrestador(int prestadorId)
//        {
//            return await context.Notificaciones
//                .Where(n => n.PrestadorId == prestadorId)
//                .OrderByDescending(n => n.FechaCreacion)
//                .ToListAsync();
//        }

//        public async Task<bool> MarcarComoLeida(int id)
//        {
//            var notificacion = await context.Notificaciones.FindAsync(id);
//            if (notificacion == null) return false;

//            notificacion.Leida = true;
//            await context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<Notificacion?> CrearNotificacionViajeAsignado(int viajeId)
//        {
//            var viaje = await context.Viajes.FindAsync(viajeId);
//            if (viaje == null) return null;

//            var notificacion = new Notificacion
//            {
//                UsuarioId = viaje.IdUsuario,
//                Titulo = "Viaje Asignado",
//                Mensaje = "Un prestador ha aceptado tu viaje",
//                Leida = false,
//                FechaCreacion = DateTime.Now
//            };

//            context.Notificaciones.Add(notificacion);
//            await context.SaveChangesAsync();
//            return notificacion;
//        }
//    }
//}


//si esto se llega a utilizar se descomenta, por ahora no se utiliza