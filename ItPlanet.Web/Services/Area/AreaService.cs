using ItPlanet.Domain.Exceptions;
using ItPlanet.Infrastructure.Repositories.Area;
using ItPlanet.Domain.Extensions;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.Web.Services.Area;

public class AreaService : IAreaService
{
    private readonly IAreaRepository _areaRepository;

    public AreaService(IAreaRepository areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public async Task<Domain.Models.Area> CreateAreaAsync(Domain.Models.Area area)
    {
        var areas = await _areaRepository.GetAllAsync().ConfigureAwait(false);
        var exisingSegments = areas.SelectMany(x => x.AreaPoints.ToSegments());
        var newSegments = area.AreaPoints.ToSegments();
        if (newSegments.Any(newSegment => exisingSegments.Any(newSegment.Intersects)))
        {
            throw new NewAreaIntersectsExistingException();
        }

        var beam = new Segment()
        {
            Start = new Point(area.AreaPoints.First().Latitude, area.AreaPoints.First().Longitude),
            End = new Point(area.AreaPoints.First().Latitude, area.AreaPoints.First().Longitude + 200)
        };
        
        if (exisingSegments.Any(exisingSegment => beam.Intersects(exisingSegment)))
        {
            throw new NewAreaInsideExistingException();
        }
        
        if (areas.Select(existingArea => new Segment()
            {
                Start = new Point(existingArea.AreaPoints.First().Latitude, existingArea.AreaPoints.First().Longitude),
                End = new Point(existingArea.AreaPoints.First().Latitude,
                    existingArea.AreaPoints.First().Longitude + 200)
            }).Any(segment => newSegments.Any(x => x.Intersects(segment))))
        {
            throw new ExistingAreaInsideNewException();
        }

        return await _areaRepository.CreateAsync(area).ConfigureAwait(false);
    }
}