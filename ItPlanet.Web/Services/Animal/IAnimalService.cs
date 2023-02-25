using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;

namespace ItPlanet.Web.Services.Animal;

public interface IAnimalService
{
    Task<Domain.Models.Animal> GetAnimalAsync(long id);
    Task<IEnumerable<Domain.Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto);
    Task<IEnumerable<VisitedPoint>> GetAnimalVisitedPoints(long animalId, VisitedLocationDto visitedLocationDto);
    Task<Domain.Models.Animal> CreateAnimalAsync(AnimalDto animalDto);
    Task<VisitedPoint> AddVisitedPointAsync(long animalId, long pointId);
    Task DeleteAnimalAsync(long animalId);
    Task<Domain.Models.Animal> AddTypeToAnimalAsync(long animalId, Domain.Models.AnimalType typeId);

    Task<Domain.Models.Animal> ReplaceAnimalTypeAsync(long animalId, Domain.Models.AnimalType oldType,
        Domain.Models.AnimalType newType);

    Task<Domain.Models.Animal> DeleteAnimalTypeFromAnimalAsync(long animalId, Domain.Models.AnimalType type);
}