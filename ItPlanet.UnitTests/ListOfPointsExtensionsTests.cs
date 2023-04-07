using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Extensions;

namespace ItPlanet.UnitTests;

public class ListOfPointsExtensionsTests
{
    [Fact]
    public void IsAllPointsOnSameLine_AllPointsOnSameLines_ReturnsTrue()
    {
        var points = new List<LocationPointDto>()
        {
            new()
            {
                Latitude = 0,
                Longitude = 0
            },
            new()
            {
                Latitude = 2,
                Longitude = 2
            },
            new()
            {
                Latitude = 4,
                Longitude = 4
            }
        };

        var allPointsOnSameLine = points.IsAllPointsOnSameLine();

        allPointsOnSameLine.Should().BeTrue();
    }

    [Fact]
    public void IsAllPointsOnSameLine_PointsNotOnSameLine_ReturnsFalse()
    {
        var points = new List<LocationPointDto>()
        {
            new()
            {
                Latitude = 0,
                Longitude = 0
            },
            new()
            {
                Latitude = 2,
                Longitude = 2
            },
            new()
            {
                Latitude = 2,
                Longitude = 5
            }
        };

        var allPointsOnSameLine = points.IsAllPointsOnSameLine();

        allPointsOnSameLine.Should().BeFalse();
    }

    [Fact]
    public void IsAllPointsOnSameLine_PointsLessThanThree_ReturnsTrue()
    {
        var points = new List<LocationPointDto>();

        var allPointsOnSameLine = points.IsAllPointsOnSameLine();

        allPointsOnSameLine.Should().BeTrue();
    }
}