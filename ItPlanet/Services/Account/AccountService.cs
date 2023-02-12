using ItPlanet.Database.Repositories.Account;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Models;

namespace ItPlanet.Services.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountModel> GetAccountAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account ?? throw new AccountNotFoundException(id);
    }

    public Task<IEnumerable<AccountModel>> SearchAsync(SearchAccountDto searchAccountDto) => _accountRepository.FindAsync(searchAccountDto);
}