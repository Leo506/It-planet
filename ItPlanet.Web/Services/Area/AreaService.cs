using System.Text.Json;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Domain.Extensions;
using ItPlanet.Domain.Geometry;
using ItPlanet.Domain.Models;
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
        var visitedPoints = await _visitedPointsRepository.GetVisitedPointsInInterval(startDate, endDate);

        var area = await GetAreaById(areaId);
        visitedPoints = GetLocationPointsInsideArea(visitedPoints, area.AreaPoints.ToSegments());

        _logger.LogInformation(nameof(GetAnalytics));
        foreach (var visitedPoint in visitedPoints)
        {
            _logger.LogInformation($"Visited point inside area: {JsonSerializer.Serialize(visitedPoint)}");
        }

        var animalsInArea = visitedPoints.Select(x => x.AnimalId).Distinct();
        animalsInArea = animalsInArea.Concat((await _animalRepository.GetAnimalsChippingInInterval(startDate, endDate))
            .Where(x => x.ChippingLocation.ToPoint().IsInside(area.AreaPoints.ToSegments()))
            .Select(x => x.Id)).Distinct();
        foreach (var l in animalsInArea)
        {
            _logger.LogInformation($"Animal in area: {l}");
        }
        var totalQuantityAnimals = animalsInArea.Count();
        
        return new AnalyticDto()
        {
            TotalQuantityAnimals = totalQuantityAnimals
        };
    }

    public IEnumerable<VisitedPoint> GetLocationPointsInsideArea(IEnumerable<VisitedPoint> points,
        IEnumerable<Segment> area)
    {
        _logger.LogInformation(nameof(GetLocationPointsInsideArea));
        _logger.LogInformation(JsonSerializer.Serialize(area));
        foreach (var p in points)
        {
            var isInside = p.LocationPoint.ToPoint().IsInside(area);
            _logger.LogInformation($"Point {JsonSerializer.Serialize(p.LocationPoint.ToPoint())} is inside? {isInside}");
        }
        return points.Where(x => x.LocationPoint.ToPoint().IsInside(area));
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