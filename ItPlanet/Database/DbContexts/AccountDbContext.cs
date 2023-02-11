using ItPlanet.Models;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.DbContexts;

public sealed class AccountDbContext : DbContext, IRepository<AccountModel, int>
{
    public DbSet<AccountModel> Accounts { get; set; }

    public AccountDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    public Task<AccountModel?> GetByIdAsync(int key) => Accounts.FirstOrDefaultAsync(x => x.Id == key);
    public async Task<IEnumerable<AccountModel>> GetByPredicate(Func<AccountModel, bool> predicate)
    {
        return Accounts.Where(predicate).ToList();
    }
}