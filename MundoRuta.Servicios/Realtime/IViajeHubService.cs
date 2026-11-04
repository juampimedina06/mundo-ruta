using MundoRuta.Shared.DTO.Realtime;

namespace MundoRuta.Servicios.Realtime
{
    public interface IViajeHubService : IAsyncDisposable
    {
        bool EstaConectado { get; }

        event Action<NuevaOfertaDTO>? NuevaOfertaRecibida;

        event Action<ViajeAsignadoDTO>? ViajeAsignado;

        event Action<EstadoViajeActualizadoDTO>? EstadoViajeActualizado;

        event Action<NuevoMensajeChatDTO>? NuevoMensajeChat;

        Task IniciarConexionAsync(CancellationToken cancellationToken = default);

        Task UnirseAViaje(int viajeId, CancellationToken cancellationToken = default);

        Task SalirDeViaje(int viajeId, CancellationToken cancellationToken = default);

        Task UnirseAPrestador(int idPrestador, CancellationToken cancellationToken = default);

        Task SalirDePrestador(int idPrestador, CancellationToken cancellationToken = default);

        Task UnirseAPasajero(int idPasajero, CancellationToken cancellationToken = default);

        Task SalirDePasajero(int idPasajero, CancellationToken cancellationToken = default);

        Task UnirseAChofer(int idChofer, CancellationToken cancellationToken = default);

        Task SalirDeChofer(int idChofer, CancellationToken cancellationToken = default);

        Task UnirseAChat(int idChat, CancellationToken cancellationToken = default);

        Task SalirDeChat(int idChat, CancellationToken cancellationToken = default);

        Task UnirseAPrestadoresDisponibles(CancellationToken cancellationToken = default);

        Task SalirDePrestadoresDisponibles(CancellationToken cancellationToken = default);
    }
}
