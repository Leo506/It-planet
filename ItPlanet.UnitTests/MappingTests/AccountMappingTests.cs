using AutoFixture;
using FluentAssertions;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;

namespace ItPlanet.UnitTests.MappingTests;

public class AccountMappingTests
{
    [Fact]
    public void Map_FromAccountDto_Success()
    {
        var mapper = MappingHelper.CreateMapper();

        var dto = new Fixture().Create<AccountDto>();

        var account = mapper.Map<Account>(dto);

        account.Should().NotBeNull();
    }
}