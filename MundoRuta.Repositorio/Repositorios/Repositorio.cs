using System;
using System.Collections.Generic;
using System.Text;
using MundoRuta.BD.Datos;
using Microsoft.EntityFrameworkCore;


namespace MundoRuta.Repositorio.Repositorios
{
    public class Repositorio<E> : IRepositorio<E> where E : class
    {
        private readonly AppDbContext context;

        public Repositorio(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<E>> Select()
        {
            return await context.Set<E>().ToListAsync();
        }

        public async Task<bool> Existe(int id)
        {
            var entity = await context.Set<E>().FindAsync(id);
            return entity != null;
        }

        public async Task<E> Insert(E entidad)
        {
            context.Set<E>().Add(entidad);
            await context.SaveChangesAsync();
            return entidad;
        }

        public async Task<E> Update(E entidad)
        {
            context.Set<E>().Update(entidad);
            await context.SaveChangesAsync();
            return entidad;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await context.Set<E>().FindAsync(id);
            if (entity == null) return false;

            context.Set<E>().Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }

        //obtener POR id
        public async Task<E> ObtenerPorId(int id)
        {
            return await context.Set<E>().FindAsync(id);
        }

    }
}
