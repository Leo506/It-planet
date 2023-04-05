namespace ItPlanet.Web.Services.Auth;

public interface IHeaderAuthenticationService
{
    Task<Domain.Models.Account?> TryLogin(string login, string password);
}