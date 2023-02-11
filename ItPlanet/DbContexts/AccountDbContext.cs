using ItPlanet.Models;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.DbContexts;

public sealed class AccountDbContext : DbContext
{
    public DbSet<AccountModel> Accounts { get; set; }

    public AccountDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}