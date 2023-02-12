namespace ItPlanet.Infrastructure.Repositories.LocationPoint;

public interface ILocationPointRepository
{
    Task<Models.LocationPoint?> GetByIdAsync(long id);
}