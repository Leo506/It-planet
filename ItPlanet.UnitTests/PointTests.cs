using FluentAssertions;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.UnitTests;

public class PointTests
{
    [Fact]
    public void IsInside_PointInsidePolygon_ReturnsTrue()
    {
        var polygon = new List<Segment>()
        {
            new() { Start = new Point(0, 0), End = new Point(0, 2) },
            new() { Start = new Point(0, 2), End = new Point(5, 2) },
            new() { Start = new Point(5, 2), End = new Point(5, 0) },
            new() { Start = new Point(5, 0), End = new Point(0, 0) }
        };

        var isInside = new Point(3, 1).IsInside(polygon);

        isInside.Should().BeTrue();
    }

    [Fact]
    public void IsInside_PointDoesNotInsidePolygon_ReturnsFalse()
    {
        var polygon = new List<Segment>()
        {
            new() { Start = new Point(0, 0), End = new Point(0, 2) },
            new() { Start = new Point(0, 2), End = new Point(5, 2) },
            new() { Start = new Point(5, 2), End = new Point(5, 0) },
            new() { Start = new Point(5, 0), End = new Point(0, 0) }
        };

        var isInside = new Point(0, 10).IsInside(polygon);

        isInside.Should().BeFalse();
    }
}