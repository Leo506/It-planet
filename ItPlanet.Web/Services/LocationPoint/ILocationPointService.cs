using ItPlanet.Domain.Dto;

namespace ItPlanet.Web.Services.LocationPoint;

public interface ILocationPointService
{
    Task<Domain.Models.LocationPoint> GetLocationPointAsync(long id);
    Task<Domain.Models.LocationPoint> CreatePointAsync(LocationPointDto pointDto);
    Task DeletePointAsync(long pointId);
    Task<Domain.Models.LocationPoint> UpdatePointAsync(long pointId, LocationPointDto pointDto);
}