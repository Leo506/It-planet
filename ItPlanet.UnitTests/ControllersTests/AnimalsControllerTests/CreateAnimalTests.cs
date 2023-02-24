using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;
using ItPlanet.Web.Controllers;
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

        var response = await sut.CreateAnimal(dto);

        response.Should().BeOfType<BadRequestResult>();
    }
}