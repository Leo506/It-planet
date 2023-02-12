namespace ItPlanet.Infrastructure.Services.LocationPoint;

public interface ILocationPointService
{
    Task<Domain.Models.LocationPoint> GetLocationPointAsync(long id);
}