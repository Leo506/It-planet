using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.LocationPoint;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.Controllers;

public class LocationsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetLocationId_LocationWasFound_Returns200([Frozen] Mock<ILocationPointService> service,
        [Greedy] LocationsController sut)
    {
        service.Setup(x => x.GetLocationPointIdAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(default(long));

        var response = await sut.GetLocationId(new LocationPointDto());

        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetLocationId_LocationNotFound_Returns404([Frozen] Mock<ILocationPointService> service,
        [Greedy] LocationsController sut)
    {
        service.Setup(x => x.GetLocationPointIdAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ThrowsAsync(new LocationPointNotFoundException());

        var response = await sut.GetLocationId(new LocationPointDto());

        response.Should().BeOfType<NotFoundResult>();
    }
}