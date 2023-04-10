using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Domain.Extensions;
using ItPlanet.Domain.Geometry;
using ItPlanet.Infrastructure.Repositories.Area;

namespace ItPlanet.Web.Services.Area;

public class AreaService : IAreaService
{
    private readonly IAreaRepository _areaRepository;
    private readonly ILogger<AreaService> _logger;

    public AreaService(IAreaRepository areaRepository, ILogger<AreaService> logger)
    {
        _areaRepository = areaRepository;
        _logger = logger;
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

    public Task<AnalyticDto> GetAnalytics(long isAny, DateTime dateTime, DateTime isAny1)
    {
        throw new NotImplementedException();
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