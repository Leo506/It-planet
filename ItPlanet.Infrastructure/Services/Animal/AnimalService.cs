using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Animal;

namespace ItPlanet.Infrastructure.Services.Animal;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;

    public AnimalService(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<Models.Animal> GetAnimalAsync(long id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        return animal ?? throw new AnimalNotFoundException(id);
    }

    public Task<IEnumerable<Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto)
    {
        return _animalRepository.SearchAsync(searchAnimalDto);
    }
}