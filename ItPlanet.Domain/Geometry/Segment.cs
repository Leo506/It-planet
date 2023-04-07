using System.Drawing;

namespace ItPlanet.Domain.Geometry;

public class Segment
{
    public Point Start { get; set; }
    
    public Point End { get; set; }

    public bool Intersects(Segment segment)
    {
        if (Start.X > End.X)
            (Start, End) = (End,Start);

        if (segment.Start.X > segment.End.X)
            (segment.Start, segment.End) = (segment.End, segment.Start);

        double k1 = 0;
        if (Start.Y != End.Y && Start.X != End.X)
            k1 = (End.Y - Start.Y) / (End.X - Start.X);

        double k2 = 0;
            
        if (segment.Start.Y != segment.End.Y && segment.Start.X != segment.End.X)
            k2 = (segment.End.Y - segment.Start.Y) / (segment.End.X - segment.Start.X);

        if (Math.Abs(k1 - k2) < 0.01)
            return false;

        if ((Start.X <= segment.End.X && segment.End.X <= End.X)
            || (Start.X <= segment.Start.X && segment.Start.X <= End.X))
            return true;

        return false;
    }
}