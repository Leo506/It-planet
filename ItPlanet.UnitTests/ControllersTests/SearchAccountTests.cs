using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Dto;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests;

public partial class AccountControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task SearchAccount_Success_Returns200([Frozen] Mock<HttpContext> httpContext,
        [Greedy] AccountsController sut)
    {
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var dto = new Fixture().Create<SearchAccountDto>();

        var response = await sut.SearchAccounts(dto);

        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task SearchAccount_InvalidAuthData_Returns401([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IHeaderAuthenticationService> authService, [Greedy] AccountsController sut)
    {
        authService.Setup(x => x.TryLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        httpContext.Setup(x => x.Request.Headers.Authorization)
            .Returns(new StringValues(GetAuthorizationHeaderValue()));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var dto = new Fixture().Build<SearchAccountDto>().With(x => x.From, 0).Create();

        var response = await sut.SearchAccounts(dto);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}