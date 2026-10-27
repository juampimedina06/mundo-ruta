using Microsoft.AspNetCore.SignalR;

namespace MundoRuta.Server.Hubs
{
    public class ViajeHub : Hub
    {
        public async Task UnirseAViaje(int viajeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"viaje_{viajeId}");
        }

        public async Task SalirDeViaje(int viajeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"viaje_{viajeId}");
        }

        public async Task UnirseAPrestador(int idPrestador)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"prestador_{idPrestador}");
        }

        public async Task SalirDePrestador(int idPrestador)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"prestador_{idPrestador}");
        }

        public async Task UnirseAPasajero(int idPasajero)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"pasajero_{idPasajero}");
        }

        public async Task SalirDePasajero(int idPasajero)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"pasajero_{idPasajero}");
        }

        public async Task UnirseAChofer(int idChofer)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chofer_{idChofer}");
        }

        public async Task SalirDeChofer(int idChofer)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chofer_{idChofer}");
        }

        public async Task UnirseAChat(int idChat)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{idChat}");
        }

        public async Task SalirDeChat(int idChat)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{idChat}");
        }

        public async Task UnirseAPrestadoresDisponibles()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "prestadores_disponibles");
        }

        public async Task SalirDePrestadoresDisponibles()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "prestadores_disponibles");
        }
    }
}
