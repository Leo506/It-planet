using ItPlanet.Dto;
using ItPlanet.Models;

namespace ItPlanet.Database.Repositories.Account;

public interface IAccountRepository
{
    Task<AccountModel?> GetByIdAsync(int id);
    Task<IEnumerable<AccountModel>> FindAsync(SearchAccountDto search);
}