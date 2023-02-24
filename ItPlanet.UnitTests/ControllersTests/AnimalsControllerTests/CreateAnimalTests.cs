using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
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
    public async Task CreateAnimal_Success_Returns201([Greedy] AnimalsController sut)
    {
        var dto = new Fixture().Build<AnimalDto>().With(x => x.Gender, GenderConstants.Male).Create();

        var response = await sut.CreateAnimal(dto).ConfigureAwait(false);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimal_DtoIsInvalid_Returns400([Greedy] AnimalsController sut)
    {
        var dto = new AnimalDto();

        var response = await sut.CreateAnimal(dto).ConfigureAwait(false);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimal_AnimalTypeNotFound_Returns404([Frozen] Mock<IAnimalService> animalService,
        [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.CreateAnimalAsync(It.IsAny<AnimalDto>()))
            .ThrowsAsync(new AnimalTypeNotFoundException(default));

        var dto = new Fixture().Build<AnimalDto>().With(x => x.Gender, GenderConstants.Male).Create();

        var response = await sut.CreateAnimal(dto).ConfigureAwait(false);

        response.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimal_ChipperIdNotFound_Returns404([Frozen] Mock<IAnimalService> animalService,
        [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.CreateAnimalAsync(It.IsAny<AnimalDto>()))
            .ThrowsAsync(new AccountNotFoundException(default));

        var dto = new Fixture().Build<AnimalDto>().With(x => x.Gender, GenderConstants.Male).Create();

        var response = await sut.CreateAnimal(dto).ConfigureAwait(false);

        response.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimal_LocationNotFound_Returns404([Frozen] Mock<IAnimalService> animalService,
        [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.CreateAnimalAsync(It.IsAny<AnimalDto>()))
            .ThrowsAsync(new LocationPointNotFoundException(default));

        var dto = new Fixture().Build<AnimalDto>().With(x => x.Gender, GenderConstants.Male).Create();

        var response = await sut.CreateAnimal(dto).ConfigureAwait(false);

        response.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimal_AnimalTypeDuplicate_Returns404([Frozen] Mock<IAnimalService> animalService,
        [Greedy] AnimalsController sut)
    {
        animalService.Setup(x => x.CreateAnimalAsync(It.IsAny<AnimalDto>()))
            .ThrowsAsync(new DuplicateAnimalTypeException());

        var dto = new Fixture().Build<AnimalDto>().With(x => x.Gender, GenderConstants.Male).Create();

        var response = await sut.CreateAnimal(dto).ConfigureAwait(false);

        response.Should().BeOfType<ConflictResult>();
    }
}