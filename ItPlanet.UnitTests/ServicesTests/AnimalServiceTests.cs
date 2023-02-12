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
    public void GetAnimalAsync_NoAnimal_ThrowAnimalNotFoundException(
        [Frozen] Mock<IAnimalRepository> repositoryMock, AnimalService sut)
    {
        repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Animal)default!);

        Assert.ThrowsAsync<AnimalNotFoundException>(() => sut.GetAnimalAsync(default!));
    }
}