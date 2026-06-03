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

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    
    }
}
