using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
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
        var animal = await _repository.GetAsync(id).ConfigureAwait(false);
        return animal ?? throw new AnimalTypeNotFoundException(id);
    }

    public async Task<Domain.Models.AnimalType> CreateTypeAsync(AnimalTypeDto typeDto)
    {
        if (await _repository.GetByType(typeDto.Type) is not null)
            throw new DuplicateAnimalTypeException();

        var typeModel = new Domain.Models.AnimalType
        {
            Type = typeDto.Type
        };

        return await _repository.CreateAsync(typeModel);
    }

    public async Task<Domain.Models.AnimalType> UpdateType(long typeId, AnimalTypeDto animalTypeDto)
    {
        await GetAnimalTypeAsync(typeId);

        if (await _repository.GetByType(animalTypeDto.Type) is not null)
            throw new DuplicateAnimalTypeException();

        var typeModel = new Domain.Models.AnimalType
        {
            Id = typeId,
            Type = animalTypeDto.Type
        };

        return await _repository.UpdateAsync(typeModel);
    }
}