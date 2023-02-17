using ItPlanet.Domain.Dto;

namespace ItPlanet.Infrastructure.Services.LocationPoint;

public interface ILocationPointService
{
    Task<Domain.Models.LocationPoint> GetLocationPointAsync(long id);
    Task<Domain.Models.LocationPoint> CreatePointAsync(LocationPointDto pointDto);
    Task DeletePointAsync(long pointId);
}