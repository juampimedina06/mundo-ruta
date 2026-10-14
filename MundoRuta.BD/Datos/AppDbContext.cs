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
        }


    }
}
