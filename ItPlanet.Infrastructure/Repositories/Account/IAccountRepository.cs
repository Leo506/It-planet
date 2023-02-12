using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Repositories.Account;

public interface IAccountRepository
{
    Task<Models.Account?> GetByIdAsync(int id);
    Task<IEnumerable<Models.Account>> FindAsync(SearchAccountDto search);
    Task<Models.Account> CreateAsync(Models.Account account);
    Task<bool> HasAccountWithEmail(string email);
}