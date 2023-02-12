using AutoFixture.Xunit2;
using ItPlanet.Database.Repositories.Animal;
using ItPlanet.Exceptions;
using ItPlanet.Models;
using ItPlanet.Services.Animal;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class AnimalServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetAnimalByIdAsync_NoAnimal_ThrowAnimalNotFoundException(
        [Frozen] Mock<IAnimalRepository> repositoryMock, AnimalService sut)
    {
        repositoryMock.Setup(x => x.GetById(It.IsAny<long>())).ReturnsAsync((AnimalModel)default!);

        Assert.ThrowsAsync<AnimalNotFoundException>(() => sut.GetAnimalByIdAsync(default!));
    }
}