using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Web.Services.Animal;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class AnimalServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetAnimalAsync_NoAnimal_ThrowAnimalNotFoundException(
        [Frozen] Mock<IAnimalRepository> repositoryMock, AnimalService sut)
    {
        repositoryMock.Setup(x => x.ExistAsync(It.IsAny<long>())).ReturnsAsync(false);

        Assert.ThrowsAsync<AnimalNotFoundException>(async () => await sut.GetAnimalAsync(default!));
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalAsync_AnimalTypeNotFound_ThrowsAnimalTypeNotFoundException(
        [Frozen] Mock<IAnimalTypeRepository> repository, AnimalService sut)
    {
        repository.Setup(x => x.ExistAsync(It.IsAny<long>())).ReturnsAsync(false);

        var dto = new AnimalDto
        {
            AnimalTypes = new long[] { 1 }
        };

        var action = async () => await sut.CreateAnimalAsync(dto);

        await action.Should().ThrowExactlyAsync<AnimalTypeNotFoundException>();
    }


    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalAsync_AccountWithChipperIdNotFound_ThrowsAccountNotFoundException(
        [Frozen] Mock<IAccountRepository> repository, AnimalService sut)
    {
        repository.Setup(x => x.ExistAsync(It.IsAny<int>())).ReturnsAsync(false);

        var dto = new AnimalDto
        {
            ChipperId = 1,
            AnimalTypes = new long[] { }
        };

        var action = async () => await sut.CreateAnimalAsync(dto);

        await action.Should().ThrowExactlyAsync<AccountNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalAsync_LocationPointNotFound_ThrowsLocationPointNotFoundException(
        [Frozen] Mock<ILocationPointRepository> locationRepository,
        [Frozen] Mock<IAnimalTypeRepository> animalTypeRepository, [Frozen] Mock<IAccountRepository> accountRepository,
        AnimalService sut)
    {
        accountRepository.Setup(x => x.ExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        animalTypeRepository.Setup(x => x.ExistAsync(It.IsAny<long>())).ReturnsAsync(true);
        locationRepository.Setup(x => x.ExistAsync(It.IsAny<long>())).ReturnsAsync(false);

        var dto = new Fixture().Create<AnimalDto>();

        var action = async () => await sut.CreateAnimalAsync(dto);

        await action.Should().ThrowExactlyAsync<LocationPointNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAnimalAsync_AnimalTypesContainsDuplications_ThrowsDuplicateAnimalTypeException(
        AnimalService sut)
    {
        var dto = new Fixture().Build<AnimalDto>().With(x => x.AnimalTypes, new long[] { 1, 1 }).Create();

        var action = async () => await sut.CreateAnimalAsync(dto);

        await action.Should().ThrowExactlyAsync<DuplicateAnimalTypeException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPointAsync_AnimalIsDead_ThrowsUnableAddPointException(
        [Frozen] Mock<IAnimalRepository> repository, AnimalService sut)
    {
        var deadAnimal = new Fixture().Build<Animal>().With(x => x.LifeStatus, LifeStatusConstants.Dead).Create();
        repository.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync(deadAnimal);

        var action = async () => await sut.AddVisitedPointAsync(1, 1);

        await action.Should().ThrowExactlyAsync<UnableAddPointException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPointAsync_AnimalIsAlreadyOnPoint_ThrowsUnableAddPointException(
        [Frozen] Mock<IAnimalRepository> repository, AnimalService sut)
    {
        const long lastVisitedPointId = 1;

        var animal = new Animal();
        animal.VisitedPoints.Add(new VisitedPoint
        {
            Id = lastVisitedPointId
        });

        repository.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync(animal);

        var action = async () => await sut.AddVisitedPointAsync(1, lastVisitedPointId);

        await action.Should().ThrowExactlyAsync<UnableAddPointException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task AddVisitedPointAsync_PointIdNotExists_ThrowsLocationPointNotFoundException(
        [Frozen] Mock<ILocationPointRepository> repository, AnimalService sut)
    {
        repository.Setup(x => x.ExistAsync(It.IsAny<long>())).ReturnsAsync(false);

        var action = async () => await sut.AddVisitedPointAsync(default, default);

        await action.Should().ThrowExactlyAsync<LocationPointNotFoundException>();
    }
}