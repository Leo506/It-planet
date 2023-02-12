using ItPlanet.Dto;
using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    private readonly ApiDbContext _dbContext;

    public AccountRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Models.Account?> GetByIdAsync(int id)
    {
        return _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Domain.Models.Account>> FindAsync(SearchAccountDto search)
    {
        return _dbContext.Accounts.Where(x =>
                x.FirstName.ToLower().Contains(search.FirstName.ToLower()) &&
                x.LastName.ToLower().Contains(search.LastName.ToLower()) &&
                x.Email.ToLower().Contains(search.Email.ToLower()))
            .Skip(search.From).Take(search.Size);
    }

    public async Task<Domain.Models.Account> CreateAsync(Domain.Models.Account account)
    {
        var result = await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public Task<bool> HasAccountWithEmail(string email)
    {
        return _dbContext.Accounts.AnyAsync(x => x.Email == email);
    }

    public Task<Domain.Models.Account?> GetByEmailAndPassword(string login, string password) => _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == login && x.Password == password);
}