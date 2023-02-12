using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Repositories.Account;

public interface IAccountRepository
{
    Task<Domain.Models.Account?> GetByIdAsync(int id);
    Task<IEnumerable<Domain.Models.Account>> FindAsync(SearchAccountDto search);
    Task<Domain.Models.Account> CreateAsync(Domain.Models.Account account);
    Task<bool> HasAccountWithEmail(string email);
    Task<Domain.Models.Account?> GetByEmailAndPassword(string login, string password);
}