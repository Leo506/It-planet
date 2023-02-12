using ItPlanet.Dto;

namespace ItPlanet.Database.Repositories.Account;

public interface IAccountRepository
{
    Task<Models.Account?> GetByIdAsync(int id);
    Task<IEnumerable<Models.Account>> FindAsync(SearchAccountDto search);
}