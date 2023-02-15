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

    public async Task<IEnumerable<Domain.Models.Account>> FindAsync(SearchAccountDto search)
    {
        return _dbContext.Accounts
            .Include(x => x.Animals)
            .Where(x =>
                x.FirstName.ToLower().Contains(search.FirstName.ToLower()) &&
                x.LastName.ToLower().Contains(search.LastName.ToLower()) &&
                x.Email.ToLower().Contains(search.Email.ToLower()))
            .Skip(search.From).Take(search.Size);
    }

    public async Task<bool> HasAccountWithEmail(string email)
    {
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);
        return account is not null;
    }

    public Task<Domain.Models.Account?> GetByEmailAndPassword(string email, string password)
    {
        return _dbContext.Accounts
            .Include(x => x.Animals)
            .FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
    }

    public Task<Domain.Models.Account?> GetByEmail(string email)
    {
        return _dbContext.Accounts
            .Include(x => x.Animals)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public Task<Domain.Models.Account?> GetAsync(int id)
    {
        return _dbContext.Accounts
            .Include(x => x.Animals)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Domain.Models.Account>> GetAllAsync()
    {
        return _dbContext.Accounts
            .Include(x => x.Animals)
            .ToListAsync();
    }

    public async Task<Domain.Models.Account> CreateAsync(Domain.Models.Account model)
    {
        var result = await _dbContext.Accounts.AddAsync(model).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return result.Entity;
    }

    public Task CreateRangeAsync(IEnumerable<Domain.Models.Account> models)
    {
        throw new NotImplementedException();
    }

    public async Task<Domain.Models.Account> UpdateAsync(Domain.Models.Account model)
    {
        var account = await GetAsync(model.Id).ConfigureAwait(false);
        account!.FirstName = model.FirstName;
        account!.LastName = model.LastName;
        account!.Email = model.Email;
        account!.Password = model.Password;

        await _dbContext.SaveChangesAsync();

        return account;
    }

    public Task UpdateRangeAsync(IEnumerable<Domain.Models.Account> models)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Domain.Models.Account model)
    {
        _dbContext.Accounts.Remove(model);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public Task DeleteRangeAsync(IEnumerable<Domain.Models.Account> models)
    {
        throw new NotImplementedException();
    }
}