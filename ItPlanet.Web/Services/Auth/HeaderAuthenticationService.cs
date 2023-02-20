using ItPlanet.Infrastructure.Repositories.Account;

namespace ItPlanet.Web.Services.Auth;

public class HeaderAuthenticationService : IHeaderAuthenticationService
{
    private readonly IAccountRepository _accountRepository;

    public HeaderAuthenticationService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> TryLogin(string login, string password)
    {
        var account = await _accountRepository.GetByEmailAndPassword(login, password);
        return account is not null;
    }
}