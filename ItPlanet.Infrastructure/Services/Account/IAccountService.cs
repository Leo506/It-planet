using ItPlanet.Dto;

namespace ItPlanet.Infrastructure.Services.Account;

public interface IAccountService
{
    Task<Domain.Models.Account> GetAccountAsync(int id);
    Task<IEnumerable<Domain.Models.Account>> SearchAsync(SearchAccountDto searchAccountDto);
    Task<Domain.Models.Account> RegisterAccountAsync(AccountDto accountDto);
    Task RemoveAccountAsync(int id);
}