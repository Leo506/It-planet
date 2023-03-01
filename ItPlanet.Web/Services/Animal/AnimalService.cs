using AutoMapper;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Infrastructure.Repositories.VisitedPoint;
using ItPlanet.Web.Extensions;

namespace ItPlanet.Web.Services.Animal;

public class AnimalService : IAnimalService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAnimalRepository _animalRepository;
    private readonly IAnimalTypeRepository _animalTypeRepository;
    private readonly ILocationPointRepository _locationPointRepository;
    private readonly IVisitedPointsRepository _visitedPointsRepository;
    private readonly IMapper _mapper;

    public AnimalService(IAnimalRepository animalRepository, IAnimalTypeRepository animalTypeRepository,
        IAccountRepository accountRepository, ILocationPointRepository locationPointRepository,
        IVisitedPointsRepository visitedPointsRepository, IMapper mapper)
    {
        _animalRepository = animalRepository;
        _animalTypeRepository = animalTypeRepository;
        _accountRepository = accountRepository;
        _locationPointRepository = locationPointRepository;
        _visitedPointsRepository = visitedPointsRepository;
        _mapper = mapper;
    }

    public async Task<Domain.Models.Animal> GetAnimalAsync(long id)
    {
        var animal = await _animalRepository.GetAsync(id);
        return animal ?? throw new AnimalNotFoundException(id);
    }

    public Task<IEnumerable<Domain.Models.Animal>> SearchAnimalAsync(SearchAnimalDto searchAnimalDto)
    {
        return _animalRepository.SearchAsync(searchAnimalDto);
    }

    public async Task<Domain.Models.Animal> CreateAnimalAsync(AnimalDto animalDto)
    {
        await EnsureAvailableCreateAnimal(animalDto);

        var animal = _mapper.Map<Domain.Models.Animal>(animalDto);
        animal.LifeStatus = LifeStatusConstants.Alive;
        animal.ChippingDateTime = DateTime.Now;

        foreach (var typeId in animalDto.AnimalTypes) animal.Types.Add((await _animalTypeRepository.GetAsync(typeId))!);

        return await _animalRepository.CreateAsync(animal);
    }

    private async Task EnsureAvailableCreateAnimal(AnimalDto animalDto)
    {
        if (animalDto.AnimalTypes.HasDuplicates())
            throw new DuplicateAnimalTypeException();

        foreach (var type in animalDto.AnimalTypes)
            if (await _animalTypeRepository.ExistAsync(type) is false)
                throw new AnimalTypeNotFoundException(type);

        if (await _accountRepository.ExistAsync(animalDto.ChipperId) is false)
            throw new AccountNotFoundException(animalDto.ChipperId);

        if (await _locationPointRepository.ExistAsync(animalDto.ChippingLocationId) is false)
            throw new LocationPointNotFoundException(animalDto.ChippingLocationId);
    }
    
    public async Task DeleteAnimalAsync(long animalId)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.VisitedPoints.Any())
            throw new UnableDeleteAnimalException();

        await _animalRepository.DeleteAsync(animal);
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

    public async Task<Domain.Models.Animal> UpdateAnimalAsync(long animalId, UpdateAnimalDto updateDto)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.LifeStatus is LifeStatusConstants.Dead && updateDto.LifeStatus is LifeStatusConstants.Alive)
            throw new UnableUpdateAnimalException();

        if (IsAttemptToSetFirstVisitedPointAsChippingPoint(animal, updateDto.ChippingLocationId))
            throw new UnableUpdateAnimalException();

        if (await _accountRepository.ExistAsync(updateDto.ChipperId) is false)
            throw new AccountNotFoundException(updateDto.ChipperId);

        if (await _locationPointRepository.ExistAsync(updateDto.ChippingLocationId) is false)
            throw new LocationPointNotFoundException(updateDto.ChippingLocationId);

        var animalModel = _mapper.Map<Domain.Models.Animal>(updateDto);
        animalModel.DeathDateTime = updateDto.LifeStatus is LifeStatusConstants.Dead ? DateTime.Now : null;
        animalModel.Id = animalId;
        animalModel.ChippingDateTime = animal.ChippingDateTime;

        return await _animalRepository.UpdateAsync(animalModel);
    }

    private static bool IsAttemptToSetFirstVisitedPointAsChippingPoint(Domain.Models.Animal animal, long locationPointId)
    {
        return animal.VisitedPoints.Any() &&
               animal.VisitedPoints.MinBy(x => x.DateTimeOfVisitLocationPoint)?.LocationPointId == locationPointId;
    }
}