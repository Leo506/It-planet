using ItPlanet.Database.Repositories.LocationPoint;
using ItPlanet.Exceptions;

namespace ItPlanet.Services.LocationPoint;

public class LocationPointService : ILocationPointService
{
    private readonly ILocationPointRepository _repository;

    public LocationPointService(ILocationPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Models.LocationPoint> GetLocationPointAsync(long id)
    {
        var point = await _repository.GetByIdAsync(id);
        return point ?? throw new LocationPointNotFoundException(id);
    }
}