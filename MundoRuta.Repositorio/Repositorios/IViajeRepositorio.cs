using MundoRuta.BD.Datos.Entity;
using MundoRuta.Shared.DTO;

namespace MundoRuta.Repositorio.Repositorios
{
    public interface IViajeRepositorio : IRepositorio<Viaje>
    {
        Task<Viaje?> ObtenerPorId(int id);
        Task InsertarConServicio(Viaje viaje);
        Task<List<Viaje>> ObtenerPorUsuario(int usuarioId, string? estado);
        Task<Viaje?> ObtenerDetalle(int viajeId, int usuarioId, bool esPrestador);
        Task<List<Viaje>> SolicitudesPendientes(int prestadorId);
        Task<List<ViajePrestadorDTO>> ListadoPorPrestador(int prestadorId, string? estado = null);
        Task<bool> ResponderSolicitud(int viajeId, ResponderSolicitudDTO dto);
        Task<bool> ContraOfertar(int viajeId, ContraOfertaDTO dto);
        Task FinalizarViaje(Viaje viaje);
        Task IniciarViaje(Viaje viaje);
        Task<bool> RegistrarPago(int viajeId, ConfirmacionPagoDTO dto);
        Task<int> ContarViajesFinalizados();

        
    }
}