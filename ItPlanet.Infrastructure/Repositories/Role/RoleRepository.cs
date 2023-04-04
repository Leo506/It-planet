using ItPlanet.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ItPlanet.Infrastructure.Repositories.Role;

public class RoleRepository : IRoleRepository
{
    private readonly ApiDbContext _dbContext;

    public RoleRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Domain.Models.Role?> GetRoleByName(string roleName)
    {
        return _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName);
    }
}