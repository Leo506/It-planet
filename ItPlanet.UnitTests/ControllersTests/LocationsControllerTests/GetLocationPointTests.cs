using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Exceptions;
using ItPlanet.UnitTests.ControllersTests.Helpers;
using ItPlanet.Web.Controllers;
using ItPlanet.Web.Services.Auth;
using ItPlanet.Web.Services.LocationPoint;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.LocationsControllerTests;

public partial class LocationsControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetLocationPoint_Success_Returns200([Frozen] Mock<HttpContext> httpContext,
        [Greedy] LocationsController sut)
    {
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetLocationPoint(1);

        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetLocationPoint_InvalidAuthData_Returns401([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IHeaderAuthenticationService> authService, [Greedy] LocationsController sut)
    {
        httpContext.Setup(x => x.Request.Headers.Authorization)
            .Returns(new StringValues(AuthHeaderHelper.GetAuthorizationHeaderValue()));

        authService.Setup(x => x.TryLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetLocationPoint(1);

        response.Should().BeOfType<UnauthorizedResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetLocationPoint_PointNotFound_Returns404([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<ILocationPointService> locationService, [Greedy] LocationsController sut)
    {
        locationService.Setup(x => x.GetLocationPointAsync(It.IsAny<long>()))
            .ThrowsAsync(new LocationPointNotFoundException(default));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetLocationPoint(1);

        response.Should().BeOfType<NotFoundResult>();
    }
}