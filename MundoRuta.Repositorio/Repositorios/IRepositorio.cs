namespace MundoRuta.Repositorio.Repositorios
{
    public interface IRepositorio<E> where E : class
    {
        Task<bool> Delete(int id);
        Task<bool> Existe(int id);
        Task<E> Insert(E entidad);
        Task<List<E>> Select();
        Task<E> Update(E entidad);

        Task<E?> ObtenerPorId(int id);
    }
}