using ItPlanet.Database.Repositories.Animal;
using ItPlanet.Exceptions;
using ItPlanet.Models;

namespace ItPlanet.Services.Animal;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;

    public AnimalService(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<AnimalModel> GetAnimalAsync(long id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        return animal ?? throw new AnimalNotFoundException(id);
    }
}