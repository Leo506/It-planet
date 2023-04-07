using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.Domain.Extensions;

public static class ListOfPointsExtensions
{
    public static bool IsAllPointsOnSameLine(this List<LocationPointDto> points)
    {
        for (var i = 2; i < points.Count; i++)
        {
            var firstPoint = points[i];
            var secondPoint = points[i - 1];
            var thirdPoint = points[i - 2];

            var left = (thirdPoint.Latitude - firstPoint.Latitude) / (secondPoint.Latitude - firstPoint.Latitude);
            var right = (thirdPoint.Longitude - firstPoint.Longitude) / (secondPoint.Longitude - firstPoint.Longitude);

            if (Math.Abs(left - right) > 0.01)
            {
                return false;
            }
        }

        return true;
    }

    public static List<Segment> ToSegments(this List<LocationPointDto> points)
    {
        var result = new List<Segment>();
        for (var i = 1; i < points.Count; i++)
        {
            result.Add(new Segment()
            {
                Start = new Point(points[i - 1].Latitude, points[i - 1].Longitude),
                End = new Point(points[i].Latitude, points[i].Longitude)
            });
        }

        return result;
    }
}