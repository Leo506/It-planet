using ItPlanet.Database.Repositories.AnimalType;
using ItPlanet.Exceptions;

namespace ItPlanet.Services.AnimalType;

public class AnimalTypeService : IAnimalTypeService
{
    private readonly IAnimalTypeRepository _repository;

    public AnimalTypeService(IAnimalTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Models.AnimalType> GetAnimalTypeAsync(long id)
    {
        var animal = await _repository.GetTypeAsync(id);
        return animal ?? throw new AnimalTypeNotFoundException(id);
    }
}