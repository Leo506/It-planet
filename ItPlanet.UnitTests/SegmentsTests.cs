using FluentAssertions;
using ItPlanet.Domain.Geometry;

namespace ItPlanet.UnitTests;

public class SegmentsTests
{
    [Fact]
    public void Intersects_ParallelLines_ReturnsFalse()
    {
        var firstSegment = new Segment()
        {
            Start = new Point(0, 0),
            End = new Point(0, 10)
        };

        var secondSegment = new Segment()
        {
            Start = new Point(10, 0),
            End = new Point(10, 10)
        };

        var isIntersects = firstSegment.Intersects(secondSegment);

        isIntersects.Should().BeFalse();
    }

    [Fact]
    public void Intersects_SegmentsIntersects_ReturnsTrue()
    {
        var firstSegment = new Segment()
        {
            Start = new Point(0, 0),
            End = new Point(3, 4)
        };

        var secondSegment = new Segment()
        {
            Start = new Point(5, 2),
            End = new Point(0, 2)
        };

        var isIntersects = firstSegment.Intersects(secondSegment);

        isIntersects.Should().BeTrue();
    }
}