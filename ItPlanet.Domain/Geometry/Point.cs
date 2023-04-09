namespace ItPlanet.Domain.Geometry;

public record Point
{
    public double X { get; set; }
    
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}