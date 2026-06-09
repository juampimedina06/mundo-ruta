using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos
{
    public class AppDbContext : DbContext
    {

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<PrestadorServicio> PrestadorServicios { get; set; }
        public DbSet<Prestador> Prestadores { get; set; }
        public DbSet<Liquidacion> Liquidaciones { get; set; }
        public DbSet<Chofer> Choferes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Chat> Chats { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    
    }
}
