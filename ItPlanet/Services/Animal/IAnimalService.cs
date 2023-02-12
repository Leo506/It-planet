using ItPlanet.Models;

namespace ItPlanet.Services.Animal;

public interface IAnimalService
{
    Task<AnimalModel> GetAnimalAsync(long id);
}