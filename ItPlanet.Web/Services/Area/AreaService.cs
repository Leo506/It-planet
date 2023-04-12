using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Domain.Extensions;
using ItPlanet.Domain.Geometry;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.Area;
using ItPlanet.Infrastructure.Repositories.VisitedPoint;

namespace ItPlanet.Web.Services.Area;

public class AreaService : IAreaService
{
    private readonly IAreaRepository _areaRepository;
    private readonly ILogger<AreaService> _logger;
    private readonly IVisitedPointsRepository _visitedPointsRepository;
    private readonly IAnimalRepository _animalRepository;

    public AreaService(IAreaRepository areaRepository, ILogger<AreaService> logger,
        IVisitedPointsRepository visitedPointsRepository, IAnimalRepository animalRepository)
    {
        _areaRepository = areaRepository;
        _logger = logger;
        _visitedPointsRepository = visitedPointsRepository;
        _animalRepository = animalRepository;
    }

    public async Task<Domain.Models.Area> GetAreaById(long id)
    {
        var area = await _areaRepository.GetAsync(id).ConfigureAwait(false);
        return area ?? throw new AreaNotFoundException();
    }

    public async Task<Domain.Models.Area> CreateAreaAsync(Domain.Models.Area area)
    {
        await EnsureCanCreateOrUpdateNewArea(area).ConfigureAwait(false);

        return await _areaRepository.CreateAsync(area).ConfigureAwait(false);
    }
    
    public async Task<Domain.Models.Area> UpdateArea(long areaId, Domain.Models.Area area)
    {
        area.Id = areaId;
        
        await EnsureCanCreateOrUpdateNewArea(area).ConfigureAwait(false);

        return await _areaRepository.UpdateAsync(area).ConfigureAwait(false);
    }

    public async Task<AnalyticDto> GetAnalytics(long areaId, DateTime startDate, DateTime endDate)
    {
        var area = await GetAreaById(areaId).ConfigureAwait(false);
        var areaSegments = area.AreaPoints.ToSegments();

        var arrivedAnimals = await GetArrivedAnimalsInArea(areaSegments, startDate, endDate)
            .ConfigureAwait(false);

        var totalAnimalsInArea = await _animalRepository.GetAnimalsChippedInArea(areaSegments, startDate, endDate)
            .ConfigureAwait(false);
        totalAnimalsInArea = totalAnimalsInArea.Concat(arrivedAnimals).DistinctBy(x => x.Id);
        
        var goneAnimals = await GetGoneAnimalsFromArea(areaSegments, startDate, endDate)
            .ConfigureAwait(false);

        var animalTypesForAnalytics = totalAnimalsInArea.Concat(arrivedAnimals).Concat(goneAnimals)
            .SelectMany(x => x.Types)
            .DistinctBy(x => x.Id);
        
        var animalsAnalytics = animalTypesForAnalytics.Select(key => new AnimalAnalytic()
            {
                AnimalType = key.Type,
                AnimalTypeId = key.Id,
                QuantityAnimals = totalAnimalsInArea.Count(x => x.Types.Contains(key)),
                AnimalsArrived = arrivedAnimals.Count(x => x.Types.Contains(key)),
                AnimalsGone = goneAnimals.Count(x => x.Types.Contains(key))
            })
            .ToList();

        return new AnalyticDto()
        {
            TotalQuantityAnimals = totalAnimalsInArea.Count(),
            TotalAnimalsArrived = arrivedAnimals.Count(),
            TotalAnimalsGone = goneAnimals.Count(),
            AnimalsAnalytics = animalsAnalytics
        };
    }

    public async Task<IEnumerable<Domain.Models.Animal>> GetArrivedAnimalsInArea(IEnumerable<Segment> area,
        DateTime startDate, DateTime endDate)
    {
        var arrivedAnimals = await _animalRepository.GetAnimalsThatVisitAreaIncludingEdge(area, startDate, endDate);
        return arrivedAnimals.DistinctBy(x => x.Id);
    }

    public async Task<IEnumerable<Domain.Models.Animal>> GetGoneAnimalsFromArea(IEnumerable<Segment> area,
        DateTime startDate, DateTime endDate)
    {
        var goneAnimals = await _animalRepository.GetAnimalsThatDoNotVisitArea(area, startDate, endDate);
        return goneAnimals.DistinctBy(x => x.Id);
    }

    private async Task EnsureCanCreateOrUpdateNewArea(Domain.Models.Area area)
    {
        await EnsureNameIsUnique(area).ConfigureAwait(false);
        
        var areas = await _areaRepository.GetAllAsync().ConfigureAwait(false);
        areas = areas.Where(x => x.Id != area.Id);
        var exisingSegments = areas.SelectMany(x => x.AreaPoints.ToSegments());
        var newSegments = area.AreaPoints.ToSegments();
        
        EnsureThereIsNoIntersectsWithExistingAreas(newSegments, exisingSegments);

        EnsureThereIsNoAreasWithSamePoints(newSegments, exisingSegments);
    }

    public async Task EnsureNameIsUnique(Domain.Models.Area area)
    {
        if (await _areaRepository.ExistAsync(area.Name))
            throw new AreaNameIsAlreadyInUsedException();
    }

    public static void EnsureThereIsNoIntersectsWithExistingAreas(IEnumerable<Segment> newAreaSegments,
        IEnumerable<Segment> exisingAreaSegments)
    {
        if (newAreaSegments.Any(newSegment => exisingAreaSegments.Any(newSegment.Intersects)))
        {
            throw new NewAreaPointsIntersectsExistingException();
        }
    }

    public static void EnsureThereIsNoAreasWithSamePoints(IEnumerable<Segment> newSegments,
        IEnumerable<Segment> exisingSegments)
    {
        if (newSegments.All(newSegment => exisingSegments.Any(newSegment.IsEqualTo)))
            throw new AreaWithSamePointHasAlreadyException();
    }

    public async Task DeleteAreaById(long areaId)
    {
        var area = await GetAreaById(areaId).ConfigureAwait(false);
        await _areaRepository.DeleteAsync(area).ConfigureAwait(false);
    }
}