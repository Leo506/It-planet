using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Services.Account;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class AccountServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAccountAsync_NoAccount_ThrowAccountNotFoundException(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Account)default!);

        await Assert.ThrowsAsync<AccountNotFoundException>(async () => await sut.GetAccountAsync(default!));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAccountAsync_ThereIdAccount_ReturnsAccount(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        var expected = new Fixture().Create<Account>();
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(expected);

        var account = await sut.GetAccountAsync(default);

        account.Should().BeSameAs(expected);
    }

    [Theory]
    [AutoMoqData]
    public async Task RegisterAccountAsync_ThereIsAccountWithSameEmail_ThrowDuplicateEmailException(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        repositoryMock.Setup(x => x.HasAccountWithEmail(It.IsAny<string>())).ReturnsAsync(true);

        var dto = new Fixture().Create<AccountDto>();

        var action = async () => await sut.RegisterAccountAsync(dto);
        await action.Should().ThrowExactlyAsync<DuplicateEmailException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateAccountAsync_NoAccountWithProvidedId_ThrowsAccountNotFoundException(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Account)default!);

        var dto = new Fixture().Create<AccountDto>();

        var action = async () => await sut.UpdateAccountAsync(default!, dto);
        await action.Should().ThrowExactlyAsync<AccountNotFoundException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task UpdateAccountAsync_NewEmailIsAlreadyUsed_ThrowsDuplicateEmailException(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        var fixture = new Fixture();

        repositoryMock.Setup(x => x.ExistAsync(It.IsAny<int>())).ReturnsAsync(true);
        repositoryMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(fixture.Create<Account>());
        repositoryMock.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(fixture.Create<Account>());

        var dto = fixture.Create<AccountDto>();
        var action = async () => await sut.UpdateAccountAsync(default!, dto);
        await action.Should().ThrowExactlyAsync<DuplicateEmailException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task EnsureEmailBelongsToAccount_EmailDoesNotBelongToAccount_ThrowsChangingNotOwnAccountException(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        const string email = "test@mail.com";
        const int firstAccountId = 1;
        const int secondAccountId = 2;

        repositoryMock.Setup(x => x.GetByEmail(email))
            .ReturnsAsync(new Fixture().Build<Account>().With(x => x.Id, firstAccountId).Create());

        var action = async () => await sut.EnsureEmailBelongsToAccount(secondAccountId, email);
        await action.Should().ThrowExactlyAsync<ChangingNotOwnAccountException>();
    }
}