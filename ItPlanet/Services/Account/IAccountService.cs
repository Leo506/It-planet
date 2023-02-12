using ItPlanet.Dto;

namespace ItPlanet.Services.Account;

public interface IAccountService
{
    Task<Models.Account> GetAccountAsync(int id);
    Task<IEnumerable<Models.Account>> SearchAsync(SearchAccountDto searchAccountDto);
    Task<Models.Account> RegisterAccountAsync(AccountDto accountDto);
}