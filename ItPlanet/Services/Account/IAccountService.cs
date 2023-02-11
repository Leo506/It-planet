using ItPlanet.Models;

namespace ItPlanet.Services.Account;

public interface IAccountService
{
    Task<AccountModel> GetAccountAsync(int id);
}