namespace ItPlanet.Infrastructure.Repositories.LocationPoint;

public interface ILocationPointRepository
{
    Task<Domain.Models.LocationPoint?> GetAsync(long id);
    Task<Domain.Models.LocationPoint> CreateAsync(Domain.Models.LocationPoint model);
    Task<Domain.Models.LocationPoint> UpdateAsync(Domain.Models.LocationPoint model);
    Task DeleteAsync(Domain.Models.LocationPoint model);
    Task<bool> ExistAsync(long id);
    Task<Domain.Models.LocationPoint?> GetPointByCoordinateAsync(double pointLatitude, double pointLongitude);
    Task<bool> HasAnimalChippedInPoint(long pointId);
}