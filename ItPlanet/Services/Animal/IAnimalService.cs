using ItPlanet.Dto;

namespace ItPlanet.Services.Animal;

public interface IAnimalService
{
    Task<Models.Animal> GetAnimalAsync(long id);
    Task<IEnumerable<Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto);
}