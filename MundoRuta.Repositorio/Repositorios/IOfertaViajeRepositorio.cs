using MundoRuta.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IOfertaViajeRepositorio : IRepositorio<OfertaViaje>
    {
        Task<int> CountActivasPorViaje(int idViaje);
        Task<List<OfertaViaje>> GetActivasPorViaje(int idViaje);
    }
}
