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

namespace ItPlanet.Web.Services.Animal;

public class AnimalService : IAnimalService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAnimalRepository _animalRepository;
    private readonly IAnimalTypeRepository _animalTypeRepository;
    private readonly ILocationPointRepository _locationPointRepository;
    private readonly IVisitedPointsRepository _visitedPointsRepository;

    public AnimalService(IAnimalRepository animalRepository, IAnimalTypeRepository animalTypeRepository,
        IAccountRepository accountRepository, ILocationPointRepository locationPointRepository,
        IVisitedPointsRepository visitedPointsRepository)
    {
        _animalRepository = animalRepository;
        _animalTypeRepository = animalTypeRepository;
        _accountRepository = accountRepository;
        _locationPointRepository = locationPointRepository;
        _visitedPointsRepository = visitedPointsRepository;
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

    public async Task<IEnumerable<VisitedPoint>> GetAnimalVisitedPoints(long animalId,
        VisitedLocationDto visitedLocationDto)
    {
        var animal = await GetAnimalAsync(animalId);
        visitedLocationDto.StarDateTime ??= DateTime.MinValue;
        visitedLocationDto.EndDateTime ??= DateTime.MaxValue;

        var visitedPoints = animal.VisitedPoints;
        var result = visitedPoints.Where(x => x.DateTimeOfVisitLocationPoint >= visitedLocationDto.StarDateTime &&
                                              x.DateTimeOfVisitLocationPoint <= visitedLocationDto.EndDateTime);

        return result.Skip(visitedLocationDto.From).Take(visitedLocationDto.Size);
    }

    public async Task<Domain.Models.Animal> CreateAnimalAsync(AnimalDto animalDto)
    {
        if (animalDto.AnimalTypes.ToHashSet().Count != animalDto.AnimalTypes.Length)
            throw new DuplicateAnimalTypeException();

        foreach (var type in animalDto.AnimalTypes)
            if (await _animalTypeRepository.ExistAsync(type) is false)
                throw new AnimalTypeNotFoundException(type);

        if (await _accountRepository.ExistAsync(animalDto.ChipperId) is false)
            throw new AccountNotFoundException(animalDto.ChipperId);

        if (await _locationPointRepository.ExistAsync(animalDto.ChippingLocationId) is false)
            throw new LocationPointNotFoundException(animalDto.ChippingLocationId);

        var animal = new Domain.Models.Animal
        {
            ChipperId = animalDto.ChipperId,
            Weight = animalDto.Weight,
            Height = animalDto.Height,
            Length = animalDto.Length,
            LifeStatus = LifeStatusConstants.Alive,
            ChippingDateTime = DateTime.Now,
            ChippingLocationId = animalDto.ChippingLocationId,
            Gender = animalDto.Gender
        };

        foreach (var typeId in animalDto.AnimalTypes) animal.Types.Add((await _animalTypeRepository.GetAsync(typeId))!);

        var newAnimal = await _animalRepository.CreateAsync(animal);

        await AddVisitedPointAsync(newAnimal.Id, animalDto.ChippingLocationId);

        return await GetAnimalAsync(newAnimal.Id);
    }

    public async Task<VisitedPoint> AddVisitedPointAsync(long animalId, long pointId)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.LifeStatus is LifeStatusConstants.Dead)
            throw new UnableAddPointException();

        if (animal.VisitedPoints.Any() &&
            animal.VisitedPoints.MaxBy(x => x.DateTimeOfVisitLocationPoint)?.Id == pointId)
            throw new UnableAddPointException();

        if (await _locationPointRepository.ExistAsync(pointId) is false)
            throw new LocationPointNotFoundException(pointId);

        var visitedPoint = new VisitedPoint
        {
            AnimalId = animalId,
            LocationPointId = pointId,
            DateTimeOfVisitLocationPoint = DateTime.Now
        };

        return await _visitedPointsRepository.CreateAsync(visitedPoint);
    }

    public async Task DeleteAnimalAsync(long animalId)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.VisitedPoints.Count >= 2)
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
}