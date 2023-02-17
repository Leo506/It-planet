using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Infrastructure.Services.LocationPoint;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class LocationPointServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetLocationPointAsync_NoPoint_ThrowLocationPointNotFoundException(
        [Frozen] Mock<ILocationPointRepository> mockRepository, LocationPointService sut)
    {
        mockRepository.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync((LocationPoint)default!);

        Assert.ThrowsAsync<LocationPointNotFoundException>(async () => await sut.GetLocationPointAsync(default));
    }

    [Theory]
    [AutoMoqData]
    public async Task CreatePointAsync_ThereIsSamePoint_ThrowsDuplicateLocationPointException(
        [Frozen] Mock<ILocationPointRepository> repository, LocationPointService sut)
    {
        var fixture = new Fixture();
        repository.Setup(x => x.GetPointByCoordinateAsync(It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(fixture.Create<LocationPoint>());

        var action = async () => await sut.CreatePointAsync(fixture.Create<LocationPointDto>());

        await action.Should().ThrowExactlyAsync<DuplicateLocationPointException>().ConfigureAwait(false);
    }
}