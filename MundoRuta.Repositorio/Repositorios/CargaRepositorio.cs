using Microsoft.EntityFrameworkCore;
using MundoRuta.BD.Datos;
using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public class CargaRepositorio : Repositorio<Carga>, ICargaRepositorio
    {
        private readonly AppDbContext context;

        public CargaRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Carga>> ObtenerPorViaje(int viajeId)
        {
            return await context.Cargas
                .Where(c => c.IdViaje == viajeId)
                .ToListAsync();
        }

        public async Task<bool> AsignarViaje(int cargaId, int viajeId)
        {
            var carga = await context.Cargas.FindAsync(cargaId);
            if (carga == null) return false;

            carga.IdViaje = viajeId;
            await context.SaveChangesAsync();
            return true;
        }
    }
}