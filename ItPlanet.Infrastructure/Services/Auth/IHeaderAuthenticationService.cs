namespace ItPlanet.Infrastructure.Services.Auth;

public interface IHeaderAuthenticationService
{
    Task<bool> TryLogin(string login, string password);
}