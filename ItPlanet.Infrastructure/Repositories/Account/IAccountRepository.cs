using ItPlanet.Domain.Dto;

namespace ItPlanet.Infrastructure.Repositories.Account;

public interface IAccountRepository
{
    Task<IEnumerable<Domain.Models.Account>> FindAsync(SearchAccountDto search);
    Task<bool> HasAccountWithEmail(string email);
    Task<Domain.Models.Account?> GetByEmailAndPassword(string email, string password);
    Task<Domain.Models.Account?> GetByEmail(string email);
    Task<Domain.Models.Account?> GetAsync(int id);
    Task CreateRangeAsync(params Domain.Models.Account[] models);
    Task<Domain.Models.Account> CreateAsync(Domain.Models.Account model);
    Task<Domain.Models.Account> UpdateAsync(Domain.Models.Account model);
    Task DeleteAsync(Domain.Models.Account model);
    Task<bool> ExistAsync(int id);
}