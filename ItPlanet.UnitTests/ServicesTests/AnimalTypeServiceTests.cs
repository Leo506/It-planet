using AutoFixture.Xunit2;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Services.AnimalType;
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