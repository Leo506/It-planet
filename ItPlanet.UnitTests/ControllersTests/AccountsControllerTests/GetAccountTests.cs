using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.Account;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.UnitTests.ControllersTests.Helpers;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AccountsControllerTests;

public partial class AccountControllerTests
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

        httpContext.Setup(x => x.Request.Headers.Authorization)
            .Returns(new StringValues(AuthHeaderHelper.GetAuthorizationHeaderValue()));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetAccount(1).ConfigureAwait(false);
        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAccount_IncorrectAuthData_Returns401([Frozen] Mock<IHeaderAuthenticationService> authService,
        [Frozen] Mock<HttpContext> httpContext, [Greedy] AccountsController sut)
    {
        authService.Setup(x => x.TryLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        httpContext.Setup(x => x.Request.Headers.Authorization)
            .Returns(new StringValues(AuthHeaderHelper.GetAuthorizationHeaderValue()));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetAccount(1);
        response.Should().BeOfType<UnauthorizedResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAccount_NoAccountWithProvidedId_Returns404([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IAccountService> accountService, [Greedy] AccountsController sut)
    {
        accountService.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ThrowsAsync(new AccountNotFoundException(1));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var response = await sut.GetAccount(1);
        response.Should().BeOfType<NotFoundResult>();
    }
}