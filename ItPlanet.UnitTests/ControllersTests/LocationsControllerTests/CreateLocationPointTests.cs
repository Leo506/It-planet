using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Infrastructure.Services.LocationPoint;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.LocationsControllerTests;

public partial class LocationsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateLocationPoint_Success_Returns201([Greedy] LocationsController sut)
    {
        var dto = new Fixture().Create<LocationPointDto>();

        var response = await sut.CreateLocationPoint(dto).ConfigureAwait(false);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateLocationPoint_ThereIsPointWithSameCoordinate_Returns409(
        [Frozen] Mock<ILocationPointService> locationService, [Greedy] LocationsController sut)
    {
        locationService.Setup(x => x.CreatePointAsync(It.IsAny<LocationPointDto>()))
            .ThrowsAsync(new DuplicateLocationPointException());

        var dto = new Fixture().Create<LocationPointDto>();

        var response = await sut.CreateLocationPoint(dto).ConfigureAwait(false);

        response.Should().BeOfType<ConflictResult>();
    }
}