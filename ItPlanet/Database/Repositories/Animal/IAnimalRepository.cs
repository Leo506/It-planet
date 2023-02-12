using ItPlanet.Dto;

namespace ItPlanet.Database.Repositories.Animal;

public interface IAnimalRepository
{
    Task<Models.Animal?> GetByIdAsync(long id);

    Task<IEnumerable<Models.Animal>> SearchAsync(SearchAnimalDto search);
}