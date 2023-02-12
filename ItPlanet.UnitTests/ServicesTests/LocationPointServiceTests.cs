using AutoFixture.Xunit2;
using ItPlanet.Database.Repositories.LocationPoint;
using ItPlanet.Exceptions;
using ItPlanet.Models;
using ItPlanet.Services.LocationPoint;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class LocationPointServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetLocationPointAsync_NoPoint_ThrowLocationPointNotFoundException(
        [Frozen] Mock<ILocationPointRepository> mockRepository, LocationPointService sut)
    {
        mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((LocationPoint)default!);

        Assert.ThrowsAsync<LocationPointNotFoundException>(async () => await sut.GetLocationPointAsync(default));
    }
}