using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;

namespace ItPlanet.Web.Services.AnimalType;

public class AnimalTypeService : IAnimalTypeService
{
    private readonly IAnimalTypeRepository _repository;
    private readonly IAnimalRepository _animalRepository;

    public AnimalTypeService(IAnimalTypeRepository repository, IAnimalRepository animalRepository)
    {
        _repository = repository;
        _animalRepository = animalRepository;
    }

    public async Task<Domain.Models.AnimalType> GetAnimalTypeAsync(long id)
    {
        var animal = await _repository.GetAsync(id).ConfigureAwait(false);
        return animal ?? throw new AnimalTypeNotFoundException(id);
    }

    public async Task<Domain.Models.AnimalType> CreateTypeAsync(AnimalTypeDto typeDto)
    {
        if (await HasType(typeDto.Type))
            throw new DuplicateAnimalTypeException();

        var typeModel = new Domain.Models.AnimalType
        {
            Type = typeDto.Type
        };

        return await _repository.CreateAsync(typeModel);
    }

    public async Task<Domain.Models.AnimalType> UpdateType(long typeId, AnimalTypeDto animalTypeDto)
    {
        await EnsureAvailableUpdateType(typeId, animalTypeDto);

        var typeModel = new Domain.Models.AnimalType
        {
            Id = typeId,
            Type = animalTypeDto.Type
        };

        return await _repository.UpdateAsync(typeModel);
    }

    private async Task EnsureAvailableUpdateType(long typeId, AnimalTypeDto animalTypeDto)
    {
        if (await HasType(typeId) is false)
            throw new AnimalTypeNotFoundException(typeId);

        if (await HasType(animalTypeDto.Type))
            throw new DuplicateAnimalTypeException();
    }

    private async Task<bool> HasType(string typeName) => await _repository.GetByType(typeName) is not null;
    private Task<bool> HasType(long typeId) => _repository.ExistAsync(typeId);

    public async Task DeleteTypeAsync(long typeId)
    {
        var type = await GetAnimalTypeAsync(typeId);

        if (HasAnimalsWithType(type))
            throw new AnimalTypeDeletionException();
        
        await _repository.DeleteAsync(type);

        bool HasAnimalsWithType(Domain.Models.AnimalType animalType) => animalType.Animals.Any();
    }
    
    public async Task<Domain.Models.Animal> AddTypeToAnimalAsync(long animalId, Domain.Models.AnimalType type)
    {
        var animal = await GetAnimalAsync(animalId);
        if (animal.AnimalTypes.Contains(type.Id))
            throw new DuplicateAnimalTypeException();

        return await _animalRepository.AddTypeAsync(animalId, type);
    }
    
    public async Task<Domain.Models.Animal> ReplaceAnimalTypeAsync(long animalId, Domain.Models.AnimalType oldType,
        Domain.Models.AnimalType newType)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.AnimalTypes.Contains(newType.Id))
            throw new DuplicateAnimalTypeException();

        if (animal.AnimalTypes.Contains(oldType.Id) is false)
            throw new AnimalTypeNotFoundException(oldType.Id);

        return await _animalRepository.ReplaceTypeAsync(animalId, oldType, newType);
    }
    
    public async Task<Domain.Models.Animal> DeleteAnimalTypeFromAnimalAsync(long animalId,
        Domain.Models.AnimalType type)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.AnimalTypes.Contains(type.Id) is false)
            throw new AnimalTypeNotFoundException(type.Id);

        if (animal.AnimalTypes.Contains(type.Id) && animal.AnimalTypes.Count() == 1)
            throw new UnableDeleteAnimalTypeException();

        return await _animalRepository.DeleteTypeAsync(animalId, type);
    }

    private async Task<Domain.Models.Animal> GetAnimalAsync(long animalId)
    {
        var animal = await _animalRepository.GetAsync(animalId);
        return animal ?? throw new AnimalNotFoundException(animalId);
    }
}