using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MundoRuta.Shared.DTO.Realtime;

namespace MundoRuta.Server.Client.Servicios
{
    public class ViajeHubService : IAsyncDisposable
    {
        private readonly NavigationManager _navigationManager;
        private HubConnection? _hubConnection;

        public event Action<NuevaOfertaDTO>? NuevaOfertaRecibida;
        public event Action<ViajeAsignadoDTO>? ViajeAsignado;
        public event Action<EstadoViajeActualizadoDTO>? EstadoViajeActualizado;
        public event Action<NuevoMensajeChatDTO>? NuevoMensajeChat;

        public bool EstaConectado => _hubConnection?.State == HubConnectionState.Connected;

        public ViajeHubService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public async Task IniciarConexionAsync()
        {
            if (_hubConnection is not null)
                return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubs/viaje"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<NuevaOfertaDTO>("NuevaOfertaRecibida", oferta => NuevaOfertaRecibida?.Invoke(oferta));
            _hubConnection.On<ViajeAsignadoDTO>("ViajeAsignado", viaje => ViajeAsignado?.Invoke(viaje));
            _hubConnection.On<EstadoViajeActualizadoDTO>("EstadoViajeActualizado", estado => EstadoViajeActualizado?.Invoke(estado));
            _hubConnection.On<NuevoMensajeChatDTO>("NuevoMensajeChat", mensaje => NuevoMensajeChat?.Invoke(mensaje));

            await _hubConnection.StartAsync();
        }

        public async Task UnirseAViaje(int viajeId)
        {
            if (_hubConnection is not null && EstaConectado)
                await _hubConnection.InvokeAsync("UnirseAViaje", viajeId);
        }

        public async Task UnirseAPrestador(int idPrestador)
        {
            if (_hubConnection is not null && EstaConectado)
                await _hubConnection.InvokeAsync("UnirseAPrestador", idPrestador);
        }

        public async Task UnirseAPasajero(int idPasajero)
        {
            if (_hubConnection is not null && EstaConectado)
                await _hubConnection.InvokeAsync("UnirseAPasajero", idPasajero);
        }

        public async Task UnirseAChofer(int idChofer)
        {
            if (_hubConnection is not null && EstaConectado)
                await _hubConnection.InvokeAsync("UnirseAChofer", idChofer);
        }

        public async Task UnirseAChat(int idChat)
        {
            if (_hubConnection is not null && EstaConectado)
                await _hubConnection.InvokeAsync("UnirseAChat", idChat);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
