using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.AnimalType;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AnimalsControllerTests;

public partial class AnimalsControllerTests
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