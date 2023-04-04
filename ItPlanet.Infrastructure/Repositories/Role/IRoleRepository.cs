namespace ItPlanet.Infrastructure.Repositories.Role;

public interface IRoleRepository
{
    Task<Domain.Models.Role?> GetRoleByName(string roleName);
}