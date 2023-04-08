namespace ItPlanet.Infrastructure.Repositories.Area;

public interface IAreaRepository
{
    Task<IEnumerable<Domain.Models.Area>> GetAllAsync();

    Task<Domain.Models.Area> CreateAsync(Domain.Models.Area areaModel);
}