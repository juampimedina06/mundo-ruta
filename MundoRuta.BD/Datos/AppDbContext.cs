using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.BD.Datos
{
    public class AppDbContext : DbContext
    {

        public DbSet<NOMBRE_ENTIDAD> NOMBRE_ENTIDADES { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    
    }
}
