using AutoFixture.Xunit2;
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
    public void GetAccountById_NoAccount_ThrowAccountNotFoundException(
        [Frozen] Mock<IRepository<AccountModel, int>> repositoryMock, AccountService sut)
    {
        repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((AccountModel)default!);

        Assert.ThrowsAsync<AccountNotFoundException>(async () => await sut.GetAccountAsync(default!));
    }
}