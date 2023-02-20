using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Exceptions;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.LocationPoint;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.LocationsControllerTests;

public partial class LocationsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task DeleteLocationPoint_Success_Returns200([Greedy] LocationsController sut)
    {
        var response = await sut.DeleteLocationPoint(1);

        response.Should().BeOfType<OkResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteLocationPoint_PointNotFound_Returns404([Frozen] Mock<ILocationPointService> locationService,
        [Greedy] LocationsController sut)
    {
        locationService.Setup(x => x.DeletePointAsync(It.IsAny<long>()))
            .ThrowsAsync(new LocationPointNotFoundException(default));

        var response = await sut.DeleteLocationPoint(1).ConfigureAwait(false);

        response.Should().BeOfType<NotFoundResult>();
    }
}