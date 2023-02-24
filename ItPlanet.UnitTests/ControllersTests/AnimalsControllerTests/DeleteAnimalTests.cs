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
    public async Task DeleteAnimal_Success_Returns200([Greedy] AnimalsController sut)
    {
        var response = await sut.DeleteAnimal(default);

        response.Should().BeOfType<OkResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteAnimal_AnimalServiceThrowsUnableDeleteAnimalException_Returns400(
        [Frozen] Mock<IAnimalService> animalService, [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.DeleteAnimalAsync(It.IsAny<long>())).ThrowsAsync(new UnableDeleteAnimalException());

        var response = await sut.DeleteAnimal(default);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteAnimal_AnimalServiceThrowsAnimalNotFoundException_Returns404(
        [Frozen] Mock<IAnimalService> animalService, [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.DeleteAnimalAsync(It.IsAny<long>()))
            .ThrowsAsync(new AnimalNotFoundException(default));

        var response = await sut.DeleteAnimal(default);

        response.Should().BeOfType<NotFoundResult>();
    }
}