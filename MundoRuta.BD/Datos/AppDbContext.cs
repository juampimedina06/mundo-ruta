using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos
{
    public class AppDbContext : DbContext
    {

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<PrestadorServicio> PrestadorServicios { get; set; }
        public DbSet<Liquidacion> Liquidaciones { get; set; }
        public DbSet<Chofer> Choferes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<OfertaViaje> OfertasViajes { get; set; }
        public DbSet<Carga> Cargas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Solicitante)
                .WithMany()
                .HasForeignKey(v => v.IdSolicitante)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Pasajero)
                .WithMany()
                .HasForeignKey(v => v.IdPasajero)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Chofer)
                .WithMany()
                .HasForeignKey(v => v.IdChofer)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chofer>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chofer>()
                .HasOne(c => c.Prestador)
                .WithMany()
                .HasForeignKey(c => c.IdPrestador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Chofer)
                .WithMany()
                .HasForeignKey(v => v.IdChofer)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.IdUsuario)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Configura la relación entre Chat y Viaje.
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Viaje) // Cada chat está relacionado con un viaje.
                .WithMany() // No se configura una colección de chats en Viaje.
                .HasForeignKey(c => c.IdViaje) // Indica la clave foránea del viaje.
                .OnDelete(DeleteBehavior.Restrict); // Impide eliminar un viaje si tiene chats relacionados.

            modelBuilder.Entity<OfertaViaje>()
                .HasOne(o => o.Viaje)
                .WithMany()
                .HasForeignKey(o => o.IdViaje)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OfertaViaje>()
                .HasOne(o => o.Prestador)
                .WithMany()
                .HasForeignKey(o => o.IdPrestador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OfertaViaje>()
                .HasOne(o => o.ChoferPropuesto)
                .WithMany()
                .HasForeignKey(o => o.IdChoferPropuesto)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
