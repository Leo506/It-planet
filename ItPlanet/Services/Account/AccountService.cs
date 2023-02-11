using ItPlanet.Database;
using ItPlanet.Exceptions;
using ItPlanet.Models;

namespace ItPlanet.Services.Account;

public class AccountService
{
    private readonly IRepository<AccountModel, int> _repository;

    public AccountService(IRepository<AccountModel, int> repository)
    {
        _repository = repository;
    }

    public async Task<AccountModel> GetAccountAsync(int id)
    {
        var account = await _repository.GetByIdAsync(id);
        return account ?? throw new AccountNotFoundException(id);
    }
}