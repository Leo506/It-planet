using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Repositories.Animal;

public interface IAnimalRepository : IRepository<Domain.Models.Animal, long>
{
    Task<IEnumerable<Domain.Models.Animal>> SearchAsync(SearchAnimalDto search);
    Task<Domain.Models.Animal> AddType(long animalId, Domain.Models.AnimalType type);
}