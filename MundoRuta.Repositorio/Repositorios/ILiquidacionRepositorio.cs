using MundoRuta.BD.Datos.Entity;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface ILiquidacionRepositorio
    {
        Task<Liquidacion> CrearLiquidacion(Liquidacion liquidacion);
        Task<List<Liquidacion>> ObtenerPendientes();
        Task<Liquidacion?> ObtenerPorId(int idLiquidacion);
        Task<List<Liquidacion>> ObtenerPorUsuarioId(int idUsuario);
        Task<List<Liquidacion>> ObtenerTodas();
    }
}