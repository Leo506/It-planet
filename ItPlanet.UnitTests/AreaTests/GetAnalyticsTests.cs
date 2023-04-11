using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Geometry;
using ItPlanet.Domain.Models;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Web.Services.Area;
using Moq;

namespace ItPlanet.UnitTests.AreaTests;

public partial class AreaServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAllAnimalsInArea_GetAnimalsThatVisitAreaInvoke(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        await sut.GetAllAnimalsInArea(default!, default!, default!);

        mockAnimalRepository.Verify(x =>
            x.GetAnimalsThatVisitArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAllAnimalsInArea_GetAnimalsChippedInAreaInvoke(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        await sut.GetAllAnimalsInArea(default!, default!, default!);

        mockAnimalRepository.Verify(x =>
            x.GetAnimalsChippedInArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAllAnimalsInArea_ReturnsAnimalsWithoutDuplicates(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        mockAnimalRepository
            .Setup(x => x.GetAnimalsThatVisitArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).ReturnsAsync(new List<Animal>()
            {
                new Animal() { Id = 1 }
            });
        mockAnimalRepository
            .Setup(x => x.GetAnimalsChippedInArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).ReturnsAsync(new List<Animal>()
            {
                new Animal() { Id = 1 }
            });

        var result = await sut.GetAllAnimalsInArea(default!, default!, default!);

        result.Should().HaveCount(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetArrivedAnimalsInArea_GetAnimalsThatVisitAreaIncludingEdgeInvoke(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        await sut.GetArrivedAnimalsInArea(default!, default!, default!);

        mockAnimalRepository.Verify(x =>
            x.GetAnimalsThatVisitAreaIncludingEdge(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>()));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetArrivedAnimalsInArea_ReturnsAnimalsWithoutDuplicates(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        mockAnimalRepository
            .Setup(x => x.GetAnimalsThatVisitAreaIncludingEdge(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).ReturnsAsync(new List<Animal>()
            {
                new() { Id = 1 },
                new() { Id = 1 }
            });

        var result = await sut.GetArrivedAnimalsInArea(default!, default!, default!);

        result.Should().HaveCount(1);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetGoneAnimalsFromArea_GetAnimalsThatDoNotVisitAreaInvoke(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        await sut.GetGoneAnimalsFromArea(default!, default!, default!);

        mockAnimalRepository.Verify(x =>
            x.GetAnimalsThatDoNotVisitArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>()));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetGoneAnimalsFromArea_GetAnimalsChippedInAreaInvoke(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        await sut.GetGoneAnimalsFromArea(default!, default!, default!);
        
        mockAnimalRepository.Verify(x =>
            x.GetAnimalsChippedInArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetGoneAnimalsFromArea_ReturnsAnimalsThatWasNotChippedInArea(
        [Frozen] Mock<IAnimalRepository> mockAnimalRepository, AreaService sut)
    {
        var firstAnimal = new Animal() { Id = 1 };
        var secondAnimal = new Animal() { Id = 2 };

        mockAnimalRepository
            .Setup(x => x.GetAnimalsThatDoNotVisitArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).ReturnsAsync(new List<Animal>()
            {
                firstAnimal,
                secondAnimal
            });

        mockAnimalRepository
            .Setup(x => x.GetAnimalsChippedInArea(It.IsAny<IEnumerable<Segment>>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).ReturnsAsync(new List<Animal>()
            {
                secondAnimal
            });

        var result = await sut.GetGoneAnimalsFromArea(default!, default!, default!);

        result.Should().HaveCount(1);
        result.Should().Contain(firstAnimal);
    }
}