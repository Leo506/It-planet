using ItPlanet.Domain.Dto;

namespace ItPlanet.Web.Services.Account;

public interface IAccountService
{
    Task<Domain.Models.Account> GetAccountAsync(int id);
    Task<IEnumerable<Domain.Models.Account>> SearchAsync(SearchAccountDto searchAccountDto);
    Task<Domain.Models.Account> RegisterAccountAsync(AccountDto accountDto);
    Task RemoveAccountAsync(int id);
    Task<Domain.Models.Account> UpdateAccountAsync(int accountId, AccountDto accountDto);
    Task EnsureEmailBelongsToAccount(int accountId, string email);
}