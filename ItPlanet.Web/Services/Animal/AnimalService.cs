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
            animal.VisitedPoints.MaxBy(x => x.DateTimeOfVisitLocationPoint)?.LocationPointId == pointId)
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

        if (animal.VisitedPoints.Count > 1)
            if (animal.VisitedPoints.OrderBy(x => x.DateTimeOfVisitLocationPoint).Skip(1).FirstOrDefault()
                    ?.LocationPointId == updateDto.ChippingLocationId)
                throw new UnableUpdateAnimalException();

        if (await _accountRepository.ExistAsync(updateDto.ChipperId) is false)
            throw new AccountNotFoundException(updateDto.ChipperId);

        if (await _locationPointRepository.ExistAsync(updateDto.ChippingLocationId) is false)
            throw new LocationPointNotFoundException(updateDto.ChippingLocationId);

        var animalModel = new Domain.Models.Animal
        {
            Id = animalId,
            ChipperId = updateDto.ChipperId,
            Height = updateDto.Height,
            Weight = updateDto.Weight,
            Length = updateDto.Length,
            ChippingLocationId = updateDto.ChippingLocationId,
            LifeStatus = updateDto.LifeStatus,
            Gender = updateDto.Gender,
            DeathDateTime = updateDto.LifeStatus is LifeStatusConstants.Dead ? DateTime.Now : null
        };

        return await _animalRepository.UpdateAsync(animalModel);
    }

    public async Task<VisitedPoint> UpdateVisitedPoint(long animalId, ReplaceVisitedPointDto replaceDto)
    {
        if (await _locationPointRepository.ExistAsync(replaceDto.LocationPointId) is false)
            throw new LocationPointNotFoundException(replaceDto.LocationPointId);

        var animal = await GetAnimalAsync(animalId);

        var visitedPoints = animal.VisitedPoints.ToList();

        for (var i = 0; i < visitedPoints.Count; i++)
        {
            if (visitedPoints[i].Id != replaceDto.VisitedLocationPointId) continue;

            if (visitedPoints[i - 1].LocationPointId == replaceDto.LocationPointId)
                throw new UnableChangeVisitedPoint();

            if (i + 1 < visitedPoints.Count && visitedPoints[i + 1].LocationPointId == replaceDto.LocationPointId)
                throw new UnableChangeVisitedPoint();

            if (visitedPoints[i].LocationPointId == replaceDto.LocationPointId)
                throw new UnableChangeVisitedPoint();

            break;
        }

        var point = visitedPoints.FirstOrDefault(x => x.Id == replaceDto.VisitedLocationPointId);
        if (point is null)
            throw new LocationPointNotFoundException(default); // TODO переработать

        var visitedPoint = new VisitedPoint
        {
            Id = replaceDto.VisitedLocationPointId,
            AnimalId = animalId,
            LocationPointId = replaceDto.LocationPointId,
            DateTimeOfVisitLocationPoint = point.DateTimeOfVisitLocationPoint
        };

        return await _visitedPointsRepository.UpdateAsync(visitedPoint);
    }
}