using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;

namespace ItPlanet.Web.Services.VisitedPoints;

public interface IVisitedPointsService
{
    Task<IEnumerable<VisitedPoint>> GetAnimalVisitedPoints(long animalId, VisitedLocationDto visitedLocationDto);

    Task<VisitedPoint> AddVisitedPointAsync(long animalId, long pointId);

    Task<VisitedPoint> UpdateVisitedPoint(long animalId, ReplaceVisitedPointDto replaceDto);

    Task DeleteVisitedPointAsync(long animalId, long visitedPointId);
}