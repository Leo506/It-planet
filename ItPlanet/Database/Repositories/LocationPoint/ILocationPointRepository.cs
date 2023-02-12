namespace ItPlanet.Database.Repositories.LocationPoint;

public interface ILocationPointRepository
{
    Task<Models.LocationPoint?> GetByIdAsync(long id);
}