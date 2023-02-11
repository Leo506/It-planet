using ItPlanet.Models;

namespace ItPlanet.Services.Account;

public interface IAccountService
{
    Task<AccountModel> GetAccountAsync(int id);
    Task<IEnumerable<AccountModel>> SearchAsync(string? firstName, string lastName, string email, int from, int size);
}