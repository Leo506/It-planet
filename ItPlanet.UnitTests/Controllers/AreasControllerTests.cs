using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.Area;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.Controllers;

public class AreasControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAnalytics_StartDateMoreThanEndDate_ReturnsBadRequest([Greedy]AreasController sut)
    {
        var endTime = DateTime.Now;
        var startTime = endTime.AddHours(1);
        var response = await sut.GetAnalytics(default, startTime, endTime);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAnalytics_StartDateEqualsEndDate_ReturnsBadRequest([Greedy] AreasController sut)
    {
        var endTime = DateTime.Now;
        var startTime = endTime;
        var response = await sut.GetAnalytics(default, startTime, endTime);

        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAnalytics_AreaServiceThrowsAreaNotFoundException_ReturnsNotFound(
        [Frozen] Mock<IAreaService> mockAreaService, [Greedy] AreasController sut)
    {
        mockAreaService.Setup(x => x.GetAnalytics(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Throws<AreaNotFoundException>();

        var response = await sut.GetAnalytics(default!, DateTime.Now, DateTime.Now.AddHours(1));

        response.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAnalytics_AllGood_ReturnsOk([Greedy] AreasController sut)
    {
        var response = await sut.GetAnalytics(default!, DateTime.Now, DateTime.Now.AddHours(1));

        response.Should().BeOfType<OkObjectResult>();
    }
}