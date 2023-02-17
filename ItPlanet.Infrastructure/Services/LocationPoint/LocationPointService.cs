using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
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

    public async Task<Domain.Models.LocationPoint> CreatePointAsync(LocationPointDto pointDto)
    {
        var point = new Domain.Models.LocationPoint
        {
            Latitude = pointDto.Latitude,
            Longitude = pointDto.Longitude
        };

        if (await _repository.GetPointByCoordinateAsync(point.Latitude, point.Longitude).ConfigureAwait(false) is not
            null)
            throw new DuplicateLocationPointException();

        return await _repository.CreateAsync(point).ConfigureAwait(false);
    }

    public async Task DeletePointAsync(long pointId)
    {
        var point = await GetLocationPointAsync(pointId);
        await _repository.DeleteAsync(point).ConfigureAwait(false);
    }
}