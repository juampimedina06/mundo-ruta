using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MundoRuta.Repositorio.Repositorios
{
    public class OfertaViajeRepositorio : Repositorio<OfertaViaje>, IOfertaViajeRepositorio
    {
        private readonly AppDbContext _contextOferta;
        public OfertaViajeRepositorio(AppDbContext context) : base(context)
        {
            _contextOferta = context;
        }

        public async Task<int> CountActivasPorViaje(int idViaje)
        {
            return await _contextOferta.OfertasViajes.CountAsync(o => o.IdViaje == idViaje && o.EstadoOferta == "ACTIVA");
        }

        public async Task<List<OfertaViaje>> GetActivasPorViaje(int idViaje)
        {
            return await _contextOferta.OfertasViajes
                .Include(o => o.Prestador)
                .Include(o => o.ChoferPropuesto)
                .Where(o => o.IdViaje == idViaje && o.EstadoOferta == "ACTIVA")
                .OrderBy(o => o.MontoOfertado)
                .ToListAsync();
        }
    }
}
