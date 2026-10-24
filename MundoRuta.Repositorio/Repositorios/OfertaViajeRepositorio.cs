using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Repositorio.Repositorios
{
    public class OfertaViajeRepositorio : Repositorio<OfertaViaje>, IOfertaViajeRepositorio
    {
        public OfertaViajeRepositorio(AppDbContext context) : base(context)
        {
        }
    }
}
