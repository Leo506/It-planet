using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.Infrastructure.Repositories.Animal;

public interface IAnimalRepository
{
    Task<IEnumerable<Domain.Models.Animal>> SearchAsync(SearchAnimalDto search);
    Task<Domain.Models.Animal> AddTypeAsync(long animalId, Domain.Models.AnimalType type);

    Task<Domain.Models.Animal> ReplaceTypeAsync(long animalId, Domain.Models.AnimalType oldType,
        Domain.Models.AnimalType newType);

    Task<Domain.Models.Animal> DeleteTypeAsync(long animalId, Domain.Models.AnimalType type);
    Task<IEnumerable<Domain.Models.Animal>> GetAnimalsChippedInArea(IEnumerable<Segment> area, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Domain.Models.Animal>> GetAnimalsThatVisitAreaIncludingEdge(IEnumerable<Segment> area, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Domain.Models.Animal>> GetGoneAnimalsFromArea(IEnumerable<Segment> area, DateTime startDate, DateTime endDate);
    Task<Domain.Models.Animal?> GetAsync(long id);
    Task<Domain.Models.Animal> CreateAsync(Domain.Models.Animal model);
    Task<Domain.Models.Animal> UpdateAsync(Domain.Models.Animal model);
    Task DeleteAsync(Domain.Models.Animal model);
}