using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.AnimalType;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AnimalsControllerTests;

public partial class AnimalsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task DeleteAnimalType_Success_Returns200([Greedy] AnimalsController sut)
    {
        var response = await sut.DeleteAnimalType(1);

        response.Should().BeOfType<OkResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteAnimalType_ThereIsAnimalWithTypeId_Returns400([Frozen] Mock<IAnimalTypeService> typeService,
        [Greedy] AnimalsController sut)
    {
        typeService.Setup(x => x.DeleteTypeAsync(It.IsAny<long>())).ThrowsAsync(new AnimalTypeDeletionException());

        var response = await sut.DeleteAnimalType(1);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteAnimalType_TypeNotFound_Returns404([Frozen] Mock<IAnimalTypeService> typeService,
        [Greedy] AnimalsController sut)
    {
        typeService.Setup(x => x.DeleteTypeAsync(It.IsAny<long>()))
            .ThrowsAsync(new AnimalTypeNotFoundException(default));

        var response = await sut.DeleteAnimalType(1);

        response.Should().BeOfType<NotFoundResult>();
    }
}