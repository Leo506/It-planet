using ItPlanet.Models;

namespace ItPlanet.Database.Repositories.Animal;

public interface IAnimalRepository
{
    Task<AnimalModel?> GetById(long id);
}