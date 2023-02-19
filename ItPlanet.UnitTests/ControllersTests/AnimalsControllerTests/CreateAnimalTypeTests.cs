using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Infrastructure.Services.AnimalType;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AnimalsControllerTests;

public class AnimalsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalType_Success_Returns201([Greedy] AnimalsController sut)
    {
        var dto = new Fixture().Create<AnimalTypeDto>();

        var response = await sut.CreateAnimalType(dto);

        response.Should().BeOfType<CreatedAtActionResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalType_TypeIsWhiteSpace_Returns400([Greedy] AnimalsController sut)
    {
        var dto = new AnimalTypeDto { Type = "" };

        var response = await sut.CreateAnimalType(dto);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalType_TypeIsAlreadyExists_Returns409([Frozen] Mock<IAnimalTypeService> typeService,
        [Greedy] AnimalsController sut)
    {
        typeService.Setup(x => x.CreateTypeAsync(It.IsAny<AnimalTypeDto>()))
            .ThrowsAsync(new DuplicateAnimalTypeException());

        var dto = new Fixture().Create<AnimalTypeDto>();

        var response = await sut.CreateAnimalType(dto);

        response.Should().BeOfType<ConflictResult>();
    }
}