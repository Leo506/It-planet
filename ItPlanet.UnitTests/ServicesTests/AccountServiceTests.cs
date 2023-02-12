using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
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
        repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Account)default!);

        await Assert.ThrowsAsync<AccountNotFoundException>(async () => await sut.GetAccountAsync(default!));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAccountAsync_ThereIdAccount_ReturnsAccount(
        [Frozen] Mock<IAccountRepository> repositoryMock, AccountService sut)
    {
        var expected = new Fixture().Create<Account>();
        repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expected);

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
}