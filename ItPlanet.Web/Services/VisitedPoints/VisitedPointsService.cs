using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Infrastructure.Repositories.VisitedPoint;
using ItPlanet.Web.Extensions;

namespace ItPlanet.Web.Services.VisitedPoints;

public class VisitedPointsService : IVisitedPointsService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IVisitedPointsRepository _visitedPointsRepository;
    private readonly ILocationPointRepository _locationPointRepository;

    public VisitedPointsService(IAnimalRepository animalRepository, IVisitedPointsRepository visitedPointsRepository,
        ILocationPointRepository locationPointRepository)
    {
        _animalRepository = animalRepository;
        _visitedPointsRepository = visitedPointsRepository;
        _locationPointRepository = locationPointRepository;
    }

    public async Task<IEnumerable<VisitedPoint>> GetAnimalVisitedPoints(long animalId,
        VisitedLocationDto visitedLocationDto)
    {
        var animal = await GetAnimalAsync(animalId);
        visitedLocationDto.StarDateTime ??= DateTime.MinValue;
        visitedLocationDto.EndDateTime ??= DateTime.MaxValue;

        var visitedPoints = animal.VisitedPoints;
        var result = visitedPoints.Where(x => x.DateTimeOfVisitLocationPoint.TrimSeconds() >= visitedLocationDto.StarDateTime &&
                                              x.DateTimeOfVisitLocationPoint.TrimSeconds() <= visitedLocationDto.EndDateTime);

        return result.Skip(visitedLocationDto.From).Take(visitedLocationDto.Size);
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

    private async Task<Domain.Models.Animal> GetAnimalAsync(long animalId)
    {
        var animal = await _animalRepository.GetAsync(animalId);
        return animal ?? throw new AnimalNotFoundException(animalId);
    }
}