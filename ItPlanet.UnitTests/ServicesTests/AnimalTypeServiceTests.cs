using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Web.Services.AnimalType;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class AnimalTypeServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetAnimalTypeAsync_NoType_ThrowAnimalTypeNotFoundException(
        [Frozen] Mock<IAnimalTypeRepository> repositoryMock, AnimalTypeService sut)
    {
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync((AnimalType)default!);

        Assert.ThrowsAsync<AnimalTypeNotFoundException>(async () => await sut.GetAnimalTypeAsync(default!));
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateType_NoTypeWithId_ThrowsAnimalTypeNotFoundException(
        [Frozen] Mock<IAnimalTypeRepository> repositoryMock, AnimalTypeService sut)
    {
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync((AnimalType)default!);

        var action = async () => await sut.UpdateType(1, new AnimalTypeDto());

        await action.Should().ThrowExactlyAsync<AnimalTypeNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateType_TypeIsAlreadyUsed_ThrowsDuplicateAnimalTypeException(
        [Frozen] Mock<IAnimalTypeRepository> repositoryMock, AnimalTypeService sut)
    {
        repositoryMock.Setup(x => x.ExistAsync(It.IsAny<long>())).ReturnsAsync(true);

        repositoryMock.Setup(x => x.GetByType(It.IsAny<string>())).ReturnsAsync(new AnimalType());

        var action = async () => await sut.UpdateType(1, new AnimalTypeDto());

        await action.Should().ThrowExactlyAsync<DuplicateAnimalTypeException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteTypeAsync_ThereIsAnimalWithType_ThrowsAnimalTypeDeletionException([Frozen] Mock<IAnimalTypeRepository> repositoryMock, AnimalTypeService sut)
    {
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync(new AnimalType()
        {
            Animals = { new Animal() }
        });

        var action = async () => await sut.DeleteTypeAsync(default);

        await action.Should().ThrowExactlyAsync<AnimalTypeDeletionException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteTypeAsync_NotTypeWithId_ThrowsAnimalTypeNotFoundException(
        [Frozen] Mock<IAnimalTypeRepository> repositoryMock, AnimalTypeService sut)
    {
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync((AnimalType)default!);

        var action = async () => await sut.DeleteTypeAsync(1);

        await action.Should().ThrowExactlyAsync<AnimalTypeNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task DeleteTypeAsync_DeleteType([Frozen] Mock<IAnimalTypeRepository> repositoryMock,
        AnimalTypeService sut)
    {
        var type = new Fixture().Create<AnimalType>();
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync(type);

        await sut.DeleteTypeAsync(default);
        
        repositoryMock.Verify(x => x.DeleteAsync(type));
    }
}