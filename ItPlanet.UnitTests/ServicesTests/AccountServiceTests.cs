using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using ItPlanet.Database;
using ItPlanet.Exceptions;
using ItPlanet.Models;
using ItPlanet.Services.Account;
using Moq;

namespace ItPlanet.UnitTests.ServicesTests;

public class AccountServiceTests
{
    [Theory]
    [AutoMoqData]
    public void GetAccountAsync_NoAccount_ThrowAccountNotFoundException(
        [Frozen] Mock<IRepository<AccountModel, int>> repositoryMock, AccountService sut)
    {
        repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((AccountModel)default!);

        Assert.ThrowsAsync<AccountNotFoundException>(async () => await sut.GetAccountAsync(default!));
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAccountAsync_ThereIdAccount_ReturnsAccount(
        [Frozen] Mock<IRepository<AccountModel, int>> repositoryMock, AccountService sut)
    {
        var expected = new Fixture().Create<AccountModel>();
        repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expected);

        var account = await sut.GetAccountAsync(default);

        account.Should().BeSameAs(expected);
    }
}