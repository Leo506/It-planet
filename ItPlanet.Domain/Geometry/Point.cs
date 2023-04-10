﻿namespace ItPlanet.Domain.Geometry;

public record Point
{
    public double X { get; set; }
    
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public bool IsInside(List<Segment> polygon)
    {
        var beam = new Segment() { Start = this, End = new Point(X + 200, Y) };
        var intersectsCount = polygon.Count(x => x.Intersects(beam));
        return intersectsCount % 2 == 1;
    }
}