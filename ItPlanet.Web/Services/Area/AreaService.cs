using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Infrastructure.Repositories.Area;
using ItPlanet.Domain.Extensions;
using ItPlanet.Domain.Geometry;

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
        if (await _areaRepository.ExistAsync(area.Name))
            throw new AreaNameIsAlreadyInUsedException();
        
        var areas = await _areaRepository.GetAllAsync().ConfigureAwait(false);
        var exisingSegments = areas.SelectMany(x => x.AreaPoints.ToSegments());
        var newSegments = area.AreaPoints.ToSegments();
        if (newSegments.Any(newSegment => exisingSegments.Any(newSegment.Intersects)))
        {
            throw new NewAreaPointsIntersectsExistingException();
        }

        if (newSegments.All(newSegment => exisingSegments.Any(newSegment.IsEqualTo)))
            throw new AreaWithSamePointHasAlreadyException();

        return await _areaRepository.CreateAsync(area).ConfigureAwait(false);
    }

    public async Task DeleteAreaById(long areaId)
    {
        var area = await GetAreaById(areaId).ConfigureAwait(false);
        await _areaRepository.DeleteAsync(area).ConfigureAwait(false);
    }
}