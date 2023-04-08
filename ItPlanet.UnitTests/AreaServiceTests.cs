using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions;
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

        await action.Should().ThrowExactlyAsync<NewAreaIntersectsExistingException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateNewArea_NewAreaInsideExistingArea_Throws([Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        var exisingArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint() { Latitude = 0, Longitude = 0 },
                new AreaPoint() { Latitude = 5, Longitude = 8 },
                new AreaPoint() { Latitude = 9, Longitude = 0 }
            }
        };

        var newArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint() { Latitude = 3, Longitude = 1 },
                new AreaPoint() { Latitude = 5, Longitude = 4 },
                new AreaPoint() { Latitude = 7, Longitude = 1 }
            }
        };

        mockAreaRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { exisingArea });

        var action = () => sut.CreateAreaAsync(newArea);

        await action.Should().ThrowExactlyAsync<NewAreaInsideExistingException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task CreateNewArea_ExistingAreaInsideNewArea_Throws([Frozen] Mock<IAreaRepository> mockAreaRepository,
        AreaService sut)
    {
        var newArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint() { Latitude = 0, Longitude = 0 },
                new AreaPoint() { Latitude = 5, Longitude = 8 },
                new AreaPoint() { Latitude = 9, Longitude = 0 }
            }
        };

        var exisingArea = new Area()
        {
            AreaPoints =
            {
                new AreaPoint() { Latitude = 3, Longitude = 1 },
                new AreaPoint() { Latitude = 5, Longitude = 4 },
                new AreaPoint() { Latitude = 7, Longitude = 1 }
            }
        };
        
        mockAreaRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { exisingArea });

        var action = () => sut.CreateAreaAsync(newArea);

        await action.Should().ThrowExactlyAsync<ExistingAreaInsideNewException>();
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
}