namespace MundoRuta.Server.Client.Auth.Services
{
    public interface IAuthClientService
    {
        Task<bool> LoginAsync(string email, string password);
        Task LogoutAsync();
    }
}
