﻿using AutoFixture.Xunit2;
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
}