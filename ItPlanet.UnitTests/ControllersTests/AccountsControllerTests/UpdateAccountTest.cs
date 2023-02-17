using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.Account;
using ItPlanet.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItPlanet.UnitTests.ControllersTests.AccountsControllerTests;

public partial class AccountControllerTests
{
    [Theory]
    [AutoMoqData]
    public async Task UpdateAccount_Success_Returns200([Frozen] Mock<HttpContext> httpContext,
        [Greedy] AccountsController sut)
    {
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var dto = new Fixture().Create<AccountDto>();
        var response = await sut.UpdateAccount(1, dto);

        response.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateAccount_UpdatingNotOwnAccount_Returns403([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IAccountService> accountService, [Greedy] AccountsController sut)
    {
        accountService.Setup(x => x.EnsureEmailBelongsToAccount(It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new ChangingNotOwnAccountException());

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var dto = new Fixture().Create<AccountDto>();

        var response = await sut.UpdateAccount(1, dto);

        response.Should().BeOfType<ForbidResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateAccount_AccountNotFound_Returns403([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IAccountService> accountService, [Greedy] AccountsController sut)
    {
        accountService.Setup(x => x.UpdateAccountAsync(It.IsAny<int>(), It.IsAny<AccountDto>()))
            .ThrowsAsync(new AccountNotFoundException(1));

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var dto = new Fixture().Create<AccountDto>();

        var response = await sut.UpdateAccount(1, dto);

        response.Should().BeOfType<ForbidResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateAccount_EmailIsAlreadyUsed_Returns409([Frozen] Mock<HttpContext> httpContext,
        [Frozen] Mock<IAccountService> accountService, [Greedy] AccountsController sut)
    {
        accountService.Setup(x => x.UpdateAccountAsync(It.IsAny<int>(), It.IsAny<AccountDto>()))
            .ThrowsAsync(new DuplicateEmailException());

        sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        var dto = new Fixture().Create<AccountDto>();

        var response = await sut.UpdateAccount(1, dto);

        response.Should().BeOfType<ConflictResult>();
    }
}