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

    public bool IsInside(IEnumerable<Segment> polygon)
    {
        var beam = new Segment() { Start = this, End = new Point(X + 200, Y) };
        var intersectsCount = polygon.Count(x => x.Intersects(beam));
        return intersectsCount % 2 == 1;
    }

    public bool IsInsideOrOnEdge(IEnumerable<Segment> polygon)
    {
        return polygon.Any(segment => IsPointOnPolygonEdge(this, segment.Start, segment.End)) || IsInside(polygon);
    }

    private static bool IsPointOnPolygonEdge(Point point, Point edgeStart, Point edgeEnd)
    {
        var distance = DistanceFromPointToLine(point, edgeStart, edgeEnd);
        return distance == 0 && IsPointBetweenEdgePoints(point, edgeStart, edgeEnd);
    }

    private static double DistanceFromPointToLine(Point point, Point lineStart, Point lineEnd)
    {
        var x1 = point.X - lineStart.X;
        var y1 = point.Y - lineStart.Y;
        var x2 = lineEnd.X - lineStart.X;
        var y2 = lineEnd.Y - lineStart.Y;

        var dotProduct = x1 * x2 + y1 * y2;
        var lineLength = x2 * x2 + y2 * y2;

        if (lineLength == 0)
        {
            return double.NaN;
        }

        var distance = dotProduct / lineLength;

        double closestX, closestY;
        switch (distance)
        {
            case < 0:
                closestX = lineStart.X;
                closestY = lineStart.Y;
                break;
            case > 1:
                closestX = lineEnd.X;
                closestY = lineEnd.Y;
                break;
            default:
                closestX = lineStart.X + distance * x2;
                closestY = lineStart.Y + distance * y2;
                break;
        }

        var deltaX = point.X - closestX;
        var deltaY = point.Y - closestY;

        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    private static bool IsPointBetweenEdgePoints(Point point, Point edgeStart, Point edgeEnd)
    {
        if (Math.Abs(edgeStart.X - edgeEnd.X) > 0.01)
        {
            return edgeStart.X <= point.X && point.X <= edgeEnd.X ||
                   edgeEnd.X <= point.X && point.X <= edgeStart.X;
        }

        return edgeStart.Y <= point.Y && point.Y <= edgeEnd.Y ||
               edgeEnd.Y <= point.Y && point.Y <= edgeStart.Y;
    }
}