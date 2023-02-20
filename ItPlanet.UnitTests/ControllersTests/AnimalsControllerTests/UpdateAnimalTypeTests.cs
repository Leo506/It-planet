using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.AnimalType;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AnimalsControllerTests;

public partial class AnimalsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task UpdateAnimalType_Success_Returns200([Greedy] AnimalsController sut)
    {
        var dto = new Fixture().Create<AnimalTypeDto>();

        var response = await sut.UpdateAnimalType(1, dto);

        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateAnimalType_TypeNotFound_Returns404([Frozen] Mock<IAnimalTypeService> typeService,
        [Greedy] AnimalsController sut)
    {
        typeService.Setup(x => x.UpdateType(It.IsAny<long>(), It.IsAny<AnimalTypeDto>()))
            .ThrowsAsync(new AnimalTypeNotFoundException(default));


        var dto = new Fixture().Create<AnimalTypeDto>();

        var response = await sut.UpdateAnimalType(1, dto);

        response.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateType_TypeIsAlreadyUsed_Returns409([Frozen] Mock<IAnimalTypeService> typeService,
        [Greedy] AnimalsController sut)
    {
        typeService.Setup(x => x.UpdateType(It.IsAny<long>(), It.IsAny<AnimalTypeDto>()))
            .ThrowsAsync(new DuplicateAnimalTypeException());

        var dto = new Fixture().Create<AnimalTypeDto>();

        var response = await sut.UpdateAnimalType(1, dto);

        response.Should().BeOfType<ConflictResult>();
    }
}