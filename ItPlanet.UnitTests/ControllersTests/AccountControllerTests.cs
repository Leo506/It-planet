using System.Text;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests;

public class AccountControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAccount_AnonymousAccess_Success_Returns200([Frozen] Mock<HttpContext> httpContext,
        [Greedy] AccountsController sut)
    {
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };
        var response = await sut.GetAccount(1).ConfigureAwait(false);
        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAccount_AuthorizedAccess_Success_Returns200([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IHeaderAuthenticationService> authServiceMock,
        [Greedy] AccountsController sut)
    {
        authServiceMock.Setup(x => x.TryLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var encodedHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes("test@test.com:password"));
        var headerValue = $"Basic {encodedHeaderValue}";

        httpContext.Setup(x => x.Request.Headers.Authorization).Returns(new StringValues(headerValue));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetAccount(1).ConfigureAwait(false);
        response.Should().BeOfType<OkObjectResult>();
    }
}