using System.Drawing;

namespace ItPlanet.Domain.Geometry;

public class Segment
{
    public Point Start { get; set; }
    
    public Point End { get; set; }

    public bool Intersects(Segment segment)
    {
        var x1 = Start.X;
        var y1 = Start.Y;
        var x2 = End.X;
        var y2 = End.Y;
        var x3 = segment.Start.X;
        var y3 = segment.Start.Y;
        var x4 = segment.End.X;
        var y4 = segment.End.Y;

        var d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
        if (d == 0) return false;

        var ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d;
        var ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d;

        if (ua is <= 0 or >= 1 || ub is <= 0 or >= 1) return false;

        return true;
    }

    private static double VectorMult(double ax, double ay, double bx, double by)
    {
        return ax * by - bx * ay;
    }

    public bool IsEqualTo(Segment segment)
    {
        return (Start.Equals(segment.Start) && End.Equals(segment.End)) ||
               (Start.Equals(segment.End) && End.Equals(segment.Start));
    }

    public override string ToString()
    {
        return $"(Start = ${Start}; End = ${End})";
    }
}