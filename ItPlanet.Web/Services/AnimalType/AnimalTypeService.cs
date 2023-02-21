using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.AnimalType;

namespace ItPlanet.Web.Services.AnimalType;

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
        if (await _repository.ExistAsync(typeId).ConfigureAwait(false) is false)
            throw new AnimalTypeNotFoundException(typeId);

        if (await _repository.GetByType(animalTypeDto.Type) is not null)
            throw new DuplicateAnimalTypeException();

        var typeModel = new Domain.Models.AnimalType
        {
            Id = typeId,
            Type = animalTypeDto.Type
        };

        return await _repository.UpdateAsync(typeModel);
    }

    public Task DeleteTypeAsync(long typeId)
    {
        throw new NotImplementedException();
    }
}