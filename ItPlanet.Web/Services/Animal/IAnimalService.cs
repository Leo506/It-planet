using ItPlanet.Domain.Dto;
using ItPlanet.Dto;

namespace ItPlanet.Web.Services.Animal;

public interface IAnimalService
{
    Task<Domain.Models.Animal> GetAnimalAsync(long id);
    Task<IEnumerable<Domain.Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto);
    Task<Domain.Models.Animal> CreateAnimalAsync(AnimalDto animalDto);
    Task DeleteAnimalAsync(long animalId);
    Task<Domain.Models.Animal> UpdateAnimalAsync(long animalId, UpdateAnimalDto updateDto);
}