using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.LocationPoint;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.LocationsControllerTests;

public partial class LocationsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task UpdateLocationPoint_Success_Returns200([Greedy] LocationsController sut)
    {
        var dto = new Fixture().Create<LocationPointDto>();

        var response = await sut.UpdateLocationPoint(1, dto);

        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateLocationPoint_PointNotFound_Returns404([Frozen] Mock<ILocationPointService> pointService,
        [Greedy] LocationsController sut)
    {
        pointService.Setup(x => x.UpdatePointAsync(It.IsAny<long>(), It.IsAny<LocationPointDto>()))
            .ThrowsAsync(new LocationPointNotFoundException(default));

        var dto = new Fixture().Create<LocationPointDto>();

        var response = await sut.UpdateLocationPoint(1, dto);

        response.Should().BeOfType<NotFoundResult>();
    }


    [Theory]
    [AutoMoqData]
    public async Task UpdateLocationPoint_ThereIsPointWithSameCoordinates_Returns409(
        [Frozen] Mock<ILocationPointService> pointService, [Greedy] LocationsController sut)
    {
        pointService.Setup(x => x.UpdatePointAsync(It.IsAny<long>(), It.IsAny<LocationPointDto>()))
            .ThrowsAsync(new DuplicateLocationPointException());

        var dto = new Fixture().Create<LocationPointDto>();

        var response = await sut.UpdateLocationPoint(1, dto);

        response.Should().BeOfType<ConflictResult>();
    }
}