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
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    
    }
}
