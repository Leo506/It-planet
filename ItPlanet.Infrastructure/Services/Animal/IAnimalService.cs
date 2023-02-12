using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Services.Animal;

public interface IAnimalService
{
    Task<Domain.Models.Animal> GetAnimalAsync(long id);
    Task<IEnumerable<Domain.Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto);
    Task<IEnumerable<VisitedPoint>> GetAnimalVisitedPoints(long animalId, VisitedLocationDto visitedLocationDto);
}