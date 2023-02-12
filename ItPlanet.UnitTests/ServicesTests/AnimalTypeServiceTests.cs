using AutoFixture.Xunit2;
using ItPlanet.Database.Repositories.AnimalType;
using ItPlanet.Exceptions;
using ItPlanet.Models;
using ItPlanet.Services.AnimalType;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class AnimalTypeServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetAnimalTypeAsync_NoType_ThrowAnimalTypeNotFoundException(
        [Frozen] Mock<IAnimalTypeRepository> repositoryMock, AnimalTypeService sut)
    {
        repositoryMock.Setup(x => x.GetTypeAsync(It.IsAny<long>())).ReturnsAsync((AnimalType)default!);

        Assert.ThrowsAsync<AnimalTypeNotFoundException>(async () => await sut.GetAnimalTypeAsync(default!));
    }
}