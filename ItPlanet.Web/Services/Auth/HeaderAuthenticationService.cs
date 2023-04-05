using ItPlanet.Infrastructure.Repositories.Account;

namespace ItPlanet.Web.Services.Auth;

public class HeaderAuthenticationService : IHeaderAuthenticationService
{
    private readonly IAccountRepository _accountRepository;

    public HeaderAuthenticationService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public Task<Domain.Models.Account?> TryLogin(string login, string password)
    {
        return _accountRepository.GetByEmailAndPassword(login, password);
    }
}