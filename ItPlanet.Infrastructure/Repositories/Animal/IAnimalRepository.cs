using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Repositories.Animal;

public interface IAnimalRepository
{
    Task<Domain.Models.Animal?> GetByIdAsync(long id);

    Task<IEnumerable<Domain.Models.Animal>> SearchAsync(SearchAnimalDto search);
}