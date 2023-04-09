using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Domain.Geometry;
using ItPlanet.Domain.Models;
using ItPlanet.Infrastructure.Repositories.Area;
using ItPlanet.Web.Services.Area;
using Moq;

namespace ItPlanet.UnitTests;

public class AreaServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task EnsureNameIsUnique_NameIsUnique_DoesNotThrows(AreaService sut)
    {
        var action = () => sut.EnsureNameIsUnique(new Area());

        await action.Should().NotThrowAsync();
    }

    [Theory]
    [AutoMoqData]
    public async Task EnsureNameIsUnique_NameDoesNotUnique_Throws([Frozen] Mock<IAreaRepository> mockRepository,
        AreaService sut)
    {
        mockRepository.Setup(x => x.ExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        var action = () => sut.EnsureNameIsUnique(new Area());

        await action.Should().ThrowExactlyAsync<AreaNameIsAlreadyInUsedException>();
    }

    [Fact]
    public void EnsureThereIsNoIntersectsWithExistingAreas_NoIntersects_DoesNotThrow()
    {
        var newAreaSegments = new List<Segment>()
        {
            new()
            {
                Start = new Point(0, 0),
                End = new Point(100, 0)
            }
        };

        var existingAreasSegments = new List<Segment>()
        {
            new()
            {
                Start = new Point(0, 10),
                End = new Point(100, 10)
            }
        };

        var action = () =>
            AreaService.EnsureThereIsNoIntersectsWithExistingAreas(newAreaSegments, existingAreasSegments);

        action.Should().NotThrow();
    }

    [Fact]
    public void EnsureThereIsNoIntersectsWithExistingAreas_ThereIsIntersects_Throws()
    {
        var newAreaSegments = new List<Segment>()
        {
            new()
            {
                Start = new Point(0, 0),
                End = new Point(3, 4)
            }
        };

        var existingAreasSegments = new List<Segment>()
        {
            new()
            {
                Start = new Point(5, 2),
                End = new Point(0, 2)
            }
        };

        var action = () =>
            AreaService.EnsureThereIsNoIntersectsWithExistingAreas(newAreaSegments, existingAreasSegments);

        action.Should().ThrowExactly<NewAreaPointsIntersectsExistingException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateNewAreaAsync_NewAreaIntersectsWithExisting_Throw(
        [Frozen] Mock<IAreaRepository> mockAreaRepository, AreaService sut)
    {
        var existingArea = new Area()
        {
            Id = 1,
            AreaPoints =
            {
                new AreaPoint() { Latitude = 0, Longitude = 0 },
                new AreaPoint() { Latitude = 2, Longitude = 3 },
                new AreaPoint() { Latitude = 3, Longitude = 0 }
            }
        };

        var newArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint() { Latitude = 2, Longitude = 1 },
                new AreaPoint() { Latitude = 4, Longitude = 3 },
                new AreaPoint() { Latitude = 5, Longitude = 1 }
            }
        };

        mockAreaRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { existingArea });

        var action = () => sut.CreateAreaAsync(newArea);

        await action.Should().ThrowExactlyAsync<NewAreaPointsIntersectsExistingException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAreaAsync_AreaIsValid_InvokeRepositoryCreateAsync([Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        var fixture = new Fixture();
        var area = new Area()
        {
            AreaPoints = { fixture.Create<AreaPoint>(), fixture.Create<AreaPoint>(), fixture.Create<AreaPoint>() }
        };

        await sut.CreateAreaAsync(area);
        
        mockAreaRepository.Verify(x => x.CreateAsync(area), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAreaAsync_AreaIsValid_ReturnsArea([Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        var fixture = new Fixture();
        var area = new Area()
        {
            AreaPoints = { fixture.Create<AreaPoint>(), fixture.Create<AreaPoint>(), fixture.Create<AreaPoint>() }
        };

        var expectedReturnArea = new Area();

        mockAreaRepository.Setup(x => x.CreateAsync(area)).ReturnsAsync(expectedReturnArea);

        var result = await sut.CreateAreaAsync(area);

        result.Should().BeSameAs(expectedReturnArea);
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateAreaAsync_ThereIsAreaWithSameName_Throws([Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        mockAreaRepository.Setup(x => x.ExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        var action = () => sut.CreateAreaAsync(new Area());

        await action.Should().ThrowExactlyAsync<AreaNameIsAlreadyInUsedException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAreaById_NoAreaWithId_Throws([Frozen] Mock<IAreaRepository> mockRepository,
        AreaService sut)
    {
        mockRepository.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync((Area)null!);

        var action = () => sut.GetAreaById(default);

        await action.Should().ThrowExactlyAsync<AreaNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAreaById_ThereIsAreaWithId_ReturnsArea([Frozen] Mock<IAreaRepository> mockRepository,
        AreaService sut)
    {
        var expectedArea = new Area();
        
        mockRepository.Setup(x => x.GetAsync(It.IsAny<long>())).ReturnsAsync(expectedArea);

        var actualArea = await sut.GetAreaById(default);

        actualArea.Should().BeSameAs(expectedArea);
    }

    [Fact]
    public void EnsureThereIsNoAreasWithSamePoints_ThereIsAreasWithSamePoints_Throws()
    {
        var segments = new List<Segment>()
        {
            new()
            {
                Start = new Point(0, 0),
                End = new Point(10, 10)
            }
        }; 

        var action = () => AreaService.EnsureThereIsNoAreasWithSamePoints(segments, segments);

        action.Should().ThrowExactly<AreaWithSamePointHasAlreadyException>();
    }
}