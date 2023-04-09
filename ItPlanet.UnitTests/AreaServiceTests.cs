using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Domain.Models;
using ItPlanet.Infrastructure.Repositories.Area;
using ItPlanet.Web.Services.Area;
using Moq;

namespace ItPlanet.UnitTests;

public class AreaServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateNewAreaAsync_NewAreaIntersectsWithExisting_Throw(
        [Frozen] Mock<IAreaRepository> mockAreaRepository, AreaService sut)
    {
        var existingArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint()
                {
                    Latitude = 0,
                    Longitude = 0
                },
                new AreaPoint()
                {
                    Latitude = 2,
                    Longitude = 3
                },
                new AreaPoint()
                {
                    Latitude = 3,
                    Longitude = 0
                }
            }
        };

        var newArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint()
                {
                    Latitude = 2,
                    Longitude = 1
                },
                new AreaPoint()
                {
                    Latitude = 4,
                    Longitude = 3
                },
                new AreaPoint()
                {
                    Latitude = 5,
                    Longitude = 1
                }
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

    /*[Theory]
    [AutoMoqData]
    public async Task CreateAreaAsync_ThereIsAreaWithSamePoints_Throws(
        [Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        var area = new Area()
        {
            AreaPoints =
            {
                new() { Latitude = 0, Longitude = 0 },
                new() { Latitude = 5, Longitude = 8 },
                new() { Latitude = 9, Longitude = 0 }
            }
        };
        
        mockAreaRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { area });

        var action = () => sut.CreateAreaAsync(area);

        await action.Should().ThrowExactlyAsync<AreaWithSamePointHasAlreadyException>();
    }*/

    /*[Theory]
    [AutoMoqData]
    public async Task CreateAreaAsync_SomePointsFromExistingAreas_CreateArea([Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        var existingArea = new Area()
        {
            AreaPoints =
            {
                new() { Latitude = -44, Longitude = -164 },
                new() { Latitude = -44, Longitude = -151 },
                new() { Latitude = -37.5, Longitude = -157.5 }
            }
        };

        var newArea = new Area()
        {
            AreaPoints =
            {
                new() { Latitude = -44, Longitude = -151 },
                new() { Latitude = -31, Longitude = -151 },
                new() { Latitude = -37.5, Longitude = -157.5 }
            }
        };
        
        mockAreaRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { existingArea });

        await sut.CreateAreaAsync(newArea).ConfigureAwait(false);
        
        mockAreaRepository.Verify(x => x.CreateAsync(newArea), Times.Once);
    }*/
}