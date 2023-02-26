namespace ItPlanet.Infrastructure.Repositories.LocationPoint;

public interface ILocationPointRepository : IRepository<Domain.Models.LocationPoint, long>
{
    Task<Domain.Models.LocationPoint?> GetPointByCoordinateAsync(double pointLatitude, double pointLongitude);
    Task<bool> HasLinkedAnimal(long pointId);
}