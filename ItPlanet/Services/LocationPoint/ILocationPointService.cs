namespace ItPlanet.Services.LocationPoint;

public interface ILocationPointService
{
    Task<Models.LocationPoint> GetLocationPointAsync(long id);
}