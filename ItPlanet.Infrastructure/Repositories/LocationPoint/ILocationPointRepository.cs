namespace ItPlanet.Infrastructure.Repositories.LocationPoint;

public interface ILocationPointRepository
{
    Task<Domain.Models.LocationPoint?> GetByIdAsync(long id);
}