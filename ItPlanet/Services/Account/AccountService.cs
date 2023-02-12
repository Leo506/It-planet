using ItPlanet.Database.Repositories.Account;
using ItPlanet.Dto;
using ItPlanet.Exceptions;

namespace ItPlanet.Services.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Models.Account> GetAccountAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account ?? throw new AccountNotFoundException(id);
    }

    public Task<IEnumerable<Models.Account>> SearchAsync(SearchAccountDto searchAccountDto)
    {
        return _accountRepository.FindAsync(searchAccountDto);
    }
}