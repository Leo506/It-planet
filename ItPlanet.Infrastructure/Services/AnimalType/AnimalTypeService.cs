using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.AnimalType;

namespace ItPlanet.Infrastructure.Services.AnimalType;

public class AnimalTypeService : IAnimalTypeService
{
    private readonly IAnimalTypeRepository _repository;

    public AnimalTypeService(IAnimalTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Models.AnimalType> GetAnimalTypeAsync(long id)
    {
        var animal = await _repository.GetTypeAsync(id);
        return animal ?? throw new AnimalTypeNotFoundException(id);
    }
}