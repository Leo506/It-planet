using AutoFixture.Xunit2;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Services.Animal;
using ItPlanet.Models;
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

        Assert.ThrowsAsync<AnimalNotFoundException>(async () => await sut.GetAnimalAsync(default!));
    }
}