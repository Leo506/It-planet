using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.LocationPoint;

namespace ItPlanet.Infrastructure.Services.LocationPoint;

public class LocationPointService : ILocationPointService
{
    private readonly ILocationPointRepository _repository;

    public LocationPointService(ILocationPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Models.LocationPoint> GetLocationPointAsync(long id)
    {
        var point = await _repository.GetAsync(id).ConfigureAwait(false);
        return point ?? throw new LocationPointNotFoundException(id);
    }
}