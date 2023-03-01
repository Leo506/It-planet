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

    public async Task<VisitedPoint> AddVisitedPointAsync(long animalId, long pointId)
    {
        var animal = await GetAnimalAsync(animalId);

        await EnsureAvailableAddVisitedPoint(pointId, animal);

        var visitedPoint = new VisitedPoint
        {
            AnimalId = animalId,
            LocationPointId = pointId,
            DateTimeOfVisitLocationPoint = DateTime.Now
        };

        return await _visitedPointsRepository.CreateAsync(visitedPoint);
    }

    private async Task EnsureAvailableAddVisitedPoint(long pointId, Domain.Models.Animal animal)
    {
        if (animal.LifeStatus is LifeStatusConstants.Dead)
            throw new UnableAddPointException();

        if (IsAttemptToAddDuplicatePoint(pointId, animal))
            throw new UnableAddPointException();

        if (IsAttemptToAddChippingPointAsFirstVisited(pointId, animal))
            throw new UnableAddPointException();

        if (await _locationPointRepository.ExistAsync(pointId) is false)
            throw new LocationPointNotFoundException(pointId);
    }

    private static bool IsAttemptToAddDuplicatePoint(long pointId, Domain.Models.Animal animal)
    {
        return animal.VisitedPoints.Any() &&
               animal.VisitedPoints.MaxBy(x => x.DateTimeOfVisitLocationPoint)?.LocationPointId == pointId;
    }
    
    private static bool IsAttemptToAddChippingPointAsFirstVisited(long pointId, Domain.Models.Animal animal)
    {
        return animal.VisitedPoints.Any() is false &&
               animal.ChippingLocationId == pointId;
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

    public async Task<VisitedPoint> UpdateVisitedPoint(long animalId, ReplaceVisitedPointDto replaceDto)
    {
        var animal = await GetAnimalAsync(animalId);
        
        await EnsureAvailableUpdateVisitedPoint(replaceDto, animal);

        var visitedPoints = animal.VisitedPoints.ToList();
        var point = visitedPoints.First(x => x.Id == replaceDto.VisitedLocationPointId);

        var visitedPoint = new VisitedPoint
        {
            Id = replaceDto.VisitedLocationPointId,
            AnimalId = animalId,
            LocationPointId = replaceDto.LocationPointId,
            DateTimeOfVisitLocationPoint = point.DateTimeOfVisitLocationPoint
        };

        return await _visitedPointsRepository.UpdateAsync(visitedPoint);
    }

    private async Task EnsureAvailableUpdateVisitedPoint(ReplaceVisitedPointDto replaceDto, Domain.Models.Animal animal)
    {
        var previousAndNextPoints =
            animal.VisitedPoints.GetPreviousAndNextElements(x => x.Id == replaceDto.VisitedLocationPointId);
        
        if (previousAndNextPoints.previous?.LocationPointId == replaceDto.LocationPointId)
            throw new UnableChangeVisitedPoint();

        if (previousAndNextPoints.previous is null && animal.ChippingLocationId == replaceDto.LocationPointId)
            throw new UnableChangeVisitedPoint();

        if (previousAndNextPoints.next?.LocationPointId == replaceDto.LocationPointId)
            throw new UnableChangeVisitedPoint();

        if (previousAndNextPoints.current is null)
            throw new LocationPointNotFoundException(replaceDto.VisitedLocationPointId);

        if (previousAndNextPoints.current.LocationPointId == replaceDto.LocationPointId)
            throw new UnableChangeVisitedPoint();

        if (await _locationPointRepository.ExistAsync(replaceDto.LocationPointId) is false)
            throw new LocationPointNotFoundException(replaceDto.LocationPointId);
    }

    public async Task DeleteVisitedPointAsync(long animalId, long visitedPointId)
    {
        var animal = await GetAnimalAsync(animalId);

        if (animal.VisitedPoints.Any(x => x.Id == visitedPointId) is false)
            throw new VisitedPointNotFoundException();

        var visitedPoints = animal.VisitedPoints.ToList();
        for (var i = 0; i < visitedPoints.Count; i++)
        {
            if (visitedPoints[i].Id != visitedPointId) continue;

            await _visitedPointsRepository.DeleteAsync(visitedPoints[i]);

            if (i + 1 < visitedPoints.Count && i == 0 &&
                visitedPoints[i + 1].LocationPointId == animal.ChippingLocationId)
                await _visitedPointsRepository.DeleteAsync(visitedPoints[i + 1]);
        }
    }
}