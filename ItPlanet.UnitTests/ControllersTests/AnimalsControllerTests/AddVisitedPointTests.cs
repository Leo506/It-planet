using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.Animal;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AnimalsControllerTests;

public partial class AnimalsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPoint_Success_Returns201([Greedy] AnimalsController sut)
    {
        var response = await sut.AddVisitedPoint(1, 1);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPoint_AnimalServiceThrowsUnableAddPointException_Returns400(
        [Frozen] Mock<IAnimalService> animalService, [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.AddVisitedPointAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new UnableAddPointException());

        var response = await sut.AddVisitedPoint(1, 1);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPoint_AnimalServiceThrowsAnimalNotFoundException_Returns404(
        [Frozen] Mock<IAnimalService> animalService, [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.AddVisitedPointAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new AnimalNotFoundException(default));

        var response = await sut.AddVisitedPoint(1, 1);

        response.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPoint_AnimalServiceThrowsLocationPointNotFoundException_Returns404(
        [Frozen] Mock<IAnimalService> animalService, [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.AddVisitedPointAsync(It.IsAny<long>(), It.IsAny<long>()))
            .ThrowsAsync(new LocationPointNotFoundException(default));

        var response = await sut.AddVisitedPoint(1, 1);

        response.Should().BeOfType<NotFoundResult>();
    }
}