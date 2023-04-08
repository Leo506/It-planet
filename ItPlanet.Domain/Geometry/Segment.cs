using System.Drawing;

namespace ItPlanet.Domain.Geometry;

public class Segment
{
    public Point Start { get; set; }
    
    public Point End { get; set; }

    public bool Intersects(Segment segment)
    {
        var v1 = VectorMult(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y, Start.X - segment.Start.X,
            Start.Y - segment.Start.Y);
        var v2 = VectorMult(segment.End.X - segment.Start.X, segment.End.Y - segment.Start.Y, End.X - segment.Start.X,
            End.Y - segment.Start.Y);
        var v3 = VectorMult(End.X - Start.X, End.Y - Start.Y, segment.Start.X - Start.X, segment.Start.Y - Start.Y);
        var v4 = VectorMult(End.X - Start.X, End.Y - Start.Y, segment.End.X - Start.X, segment.End.Y - Start.Y);

        if (v1 * v2 < 0 && v3 * v4 < 0)
            return true;
        return false;
    }

    private double VectorMult(double ax, double ay, double bx, double by)
    {
        return ax * by - bx * ay;
    }
}