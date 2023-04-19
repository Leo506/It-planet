using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Extensions;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.Domain.Dto;

public class CreateAreaDto
{
    [Required] [MinLength(1)] public string Name { get; set; } = default!;

    [Required] [MinLength(3)] public List<LocationPointDto> AreaPoints { get; set; } = default!;

    public bool IsValidArea()
    {
        if (AreaPoints.Contains(null!))
            return false;

        if (IsTherePointDuplicates()) 
            return false;

        if (AreaPoints.IsAllPointsOnSameLine())
            return false;

        return IsThereIntersects() is false;
    }

    private bool IsTherePointDuplicates() => 
        AreaPoints.Select(x => new Point(x.Latitude, x.Longitude)).HasDuplicates();

    private bool IsThereIntersects()
    {
        var segments = AreaPoints.ToSegments();

        for (var i = 0; i < segments.Count; i++)
        {
            for (var j = i; j < segments.Count; j++)
            {
                if (segments[i].Intersects(segments[j]))
                    return true;
            }
        }

        return false;
    }
}