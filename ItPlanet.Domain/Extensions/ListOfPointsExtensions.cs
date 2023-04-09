using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Geometry;
using ItPlanet.Domain.Models;

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

            if (ArePointsOnLine(firstPoint, secondPoint, thirdPoint) is false)
                return false;
        }

        return true;
    }
    
    private static bool ArePointsOnLine(LocationPointDto p1, LocationPointDto p2, LocationPointDto p3)
    {
        var angle1 = Math.Atan2(p2.Longitude - p1.Longitude, p2.Latitude - p1.Latitude);
        var angle2 = Math.Atan2(p3.Longitude - p2.Longitude, p3.Latitude - p2.Latitude);

        return Math.Abs(angle1 - angle2) < 0.01;
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
        
        result.Add(new Segment()
        {
            Start = new Point(points.Last().Latitude, points.Last().Longitude),
            End = new Point(points.First().Latitude, points.First().Longitude)
        });

        return result;
    }

    public static List<Segment> ToSegments(this ICollection<AreaPoint> points)
    {
        if (points.Any() is false)
            return new List<Segment>();
        
        var result = new List<Segment>();
        for (var i = 1; i < points.Count; i++)
        {
            result.Add(new Segment()
            {
                Start = new Point(points.ElementAt(i - 1).Latitude, points.ElementAt(i - 1).Longitude),
                End = new Point(points.ElementAt(i).Latitude, points.ElementAt(i).Longitude)
            });   
        }
        
        result.Add(new Segment()
        {
            Start = new Point(points.Last().Latitude, points.Last().Longitude),
            End = new Point(points.First().Latitude, points.First().Longitude)
        });

        return result;
    }
}