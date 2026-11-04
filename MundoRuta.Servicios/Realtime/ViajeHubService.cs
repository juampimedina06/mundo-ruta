using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using MundoRuta.Shared.DTO.Realtime;

namespace MundoRuta.Servicios.Realtime
{
    public sealed class ViajeHubService : IViajeHubService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViajeHubService> _logger;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private HubConnection? _hubConnection;
        private bool _disposed;

        private const string HubPath = "hubs/viaje";

        public event Action<NuevaOfertaDTO>? NuevaOfertaRecibida;

        public event Action<ViajeAsignadoDTO>? ViajeAsignado;

        public event Action<EstadoViajeActualizadoDTO>? EstadoViajeActualizado;

        public event Action<NuevoMensajeChatDTO>? NuevoMensajeChat;

        public bool EstaConectado => _hubConnection?.State == HubConnectionState.Connected;

        public ViajeHubService(HttpClient httpClient, ILogger<ViajeHubService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task IniciarConexionAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ViajeHubService));
            }

            if (EstaConectado)
            {
                return;
            }

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(ViajeHubService));
                }

                if (EstaConectado)
                {
                    return;
                }

                if (_hubConnection is not null && _hubConnection.State != HubConnectionState.Disconnected)
                {
                    return;
                }

                if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }

                Uri hubUri = ConstruirUriHub();

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUri)
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.Reconnecting += error =>
                {
                    _logger.LogWarning(error, "Reconexión al hub en curso.");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += connectionId =>
                {
                    _logger.LogInformation("Reconexión al hub exitosa. ConnectionId: {ConnectionId}", connectionId);
                    return Task.CompletedTask;
                };

                _hubConnection.Closed += error =>
                {
                    _logger.LogWarning(error, "Conexión con el hub cerrada.");
                    return Task.CompletedTask;
                };

                _hubConnection.On<NuevaOfertaDTO>("NuevaOfertaRecibida", oferta => NuevaOfertaRecibida?.Invoke(oferta));
                _hubConnection.On<ViajeAsignadoDTO>("ViajeAsignado", viaje => ViajeAsignado?.Invoke(viaje));
                _hubConnection.On<EstadoViajeActualizadoDTO>("EstadoViajeActualizado", estado => EstadoViajeActualizado?.Invoke(estado));
                _hubConnection.On<NuevoMensajeChatDTO>("NuevoMensajeChat", mensaje => NuevoMensajeChat?.Invoke(mensaje));

                try
                {
                    await _hubConnection.StartAsync(cancellationToken);
                    _logger.LogInformation("Conexión con el hub iniciada correctamente.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "No se pudo iniciar la conexión con el hub.");
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                    throw;
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public Task UnirseAViaje(int viajeId, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("UnirseAViaje", viajeId, cancellationToken);
        }

        public Task SalirDeViaje(int viajeId, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("SalirDeViaje", viajeId, cancellationToken);
        }

        public Task UnirseAPrestador(int idPrestador, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("UnirseAPrestador", idPrestador, cancellationToken);
        }

        public Task SalirDePrestador(int idPrestador, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("SalirDePrestador", idPrestador, cancellationToken);
        }

        public Task UnirseAPasajero(int idPasajero, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("UnirseAPasajero", idPasajero, cancellationToken);
        }

        public Task SalirDePasajero(int idPasajero, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("SalirDePasajero", idPasajero, cancellationToken);
        }

        public Task UnirseAChofer(int idChofer, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("UnirseAChofer", idChofer, cancellationToken);
        }

        public Task SalirDeChofer(int idChofer, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("SalirDeChofer", idChofer, cancellationToken);
        }

        public Task UnirseAChat(int idChat, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("UnirseAChat", idChat, cancellationToken);
        }

        public Task SalirDeChat(int idChat, CancellationToken cancellationToken = default)
        {
            return InvocarAsync("SalirDeChat", idChat, cancellationToken);
        }

        public Task UnirseAPrestadoresDisponibles(CancellationToken cancellationToken = default)
        {
            return InvocarAsync("UnirseAPrestadoresDisponibles", cancellationToken);
        }

        public Task SalirDePrestadoresDisponibles(CancellationToken cancellationToken = default)
        {
            return InvocarAsync("SalirDePrestadoresDisponibles", cancellationToken);
        }

        private async Task InvocarAsync(string metodo, CancellationToken cancellationToken)
        {
            await IniciarConexionAsync(cancellationToken);
            if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("No se pudo inicializar la conexión con el hub.");
            }

            await _hubConnection.InvokeAsync(metodo, cancellationToken);
        }

        private async Task InvocarAsync(string metodo, int parametro, CancellationToken cancellationToken)
        {
            await IniciarConexionAsync(cancellationToken);
            if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("No se pudo inicializar la conexión con el hub.");
            }

            await _hubConnection.InvokeAsync(metodo, parametro, cancellationToken);
        }

        private Uri ConstruirUriHub()
        {
            if (_httpClient.BaseAddress is null)
            {
                throw new InvalidOperationException("No se pudo inicializar la conexión con el hub.");
            }

            string baseAddress = _httpClient.BaseAddress.ToString();
            if (!baseAddress.EndsWith("/"))
            {
                baseAddress = baseAddress + "/";
            }

            Uri uriBase = new Uri(baseAddress);
            Uri uriHub = new Uri(uriBase, HubPath);
            return uriHub;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            await _connectionLock.WaitAsync();
            try
            {
                if (_hubConnection is not null)
                {
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
    }
}
