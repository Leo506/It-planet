using ItPlanet.Database.DbContexts;
using ItPlanet.Dto;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    private readonly ApiDbContext _dbContext;

    public AccountRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Models.Account?> GetByIdAsync(int id)
    {
        return _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Models.Account>> FindAsync(SearchAccountDto search)
    {
        return _dbContext.Accounts.Where(x =>
                x.FirstName.ToLower().Contains(search.FirstName.ToLower()) &&
                x.LastName.ToLower().Contains(search.LastName.ToLower()) &&
                x.Email.ToLower().Contains(search.Email.ToLower()))
            .Skip(search.From).Take(search.Size);
    }
}