using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Geometry;
using ItPlanet.Domain.Models;
using ItPlanet.Infrastructure.Repositories.VisitedPoint;
using ItPlanet.Web.Services.Area;
using Moq;

namespace ItPlanet.UnitTests.AreaTests;

public partial class AreaServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAnalytics_InvokeGetVisitedPointsInInterval(
        [Frozen] Mock<IVisitedPointsRepository> mockRepository, AreaService sut)
    {
        await sut.GetAnalytics(default, default, default);

        mockRepository.Verify(x => x.GetVisitedPointsInInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public void GetLocationPointsInsideArea_ReturnsLocationsPointsInsideArea(AreaService sut)
    {
        var area = new List<Segment>()
        {
            new() { Start = new Point(0, 0), End = new Point(0, 2) },
            new() { Start = new Point(0, 2), End = new Point(5, 2) },
            new() { Start = new Point(5, 2), End = new Point(5, 0) },
            new() { Start = new Point(5, 0), End = new Point(0, 0) }
        };

        var locationPoints = new List<VisitedPoint>()
        {
            new() { LocationPoint = new() { Latitude = 3, Longitude = 1 } },
            new() { LocationPoint = new() { Latitude = 0, Longitude = 10 } }
        };

        var result = sut.GetLocationPointsInsideArea(locationPoints, area);

        result.Should().HaveCount(1);
    }
}