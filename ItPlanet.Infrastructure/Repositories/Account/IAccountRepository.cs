using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Repositories.Account;

public interface IAccountRepository : IRepository<Domain.Models.Account, int>
{
    Task<IEnumerable<Domain.Models.Account>> FindAsync(SearchAccountDto search);
    Task<bool> HasAccountWithEmail(string email);
    Task<Domain.Models.Account?> GetByEmailAndPassword(string email, string password);
}