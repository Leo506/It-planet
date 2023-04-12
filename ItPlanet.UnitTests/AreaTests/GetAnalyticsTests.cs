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
}