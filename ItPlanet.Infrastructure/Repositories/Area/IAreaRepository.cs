namespace ItPlanet.Infrastructure.Repositories.Area;

public interface IAreaRepository
{
    Task<IEnumerable<Domain.Models.Area>> GetAllAsync();

    Task<Domain.Models.Area> CreateAsync(Domain.Models.Area areaModel);
    Task<bool> ExistAsync(string areaName);
    Task<Domain.Models.Area?> GetAsync(long id);
    Task DeleteAsync(Domain.Models.Area area);
    Task<Domain.Models.Area> UpdateAsync(Domain.Models.Area area);
}