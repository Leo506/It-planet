using ItPlanet.Models;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Database.DbContexts;

public sealed class ApiDbContext : DbContext
{
    public DbSet<AccountModel> Accounts { get; set; }

    public ApiDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
}