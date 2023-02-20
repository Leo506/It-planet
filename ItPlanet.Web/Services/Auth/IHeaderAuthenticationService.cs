namespace ItPlanet.Web.Services.Auth;

public interface IHeaderAuthenticationService
{
    Task<bool> TryLogin(string login, string password);
}