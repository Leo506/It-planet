using ItPlanet.Domain.Dto;

namespace ItPlanet.Web.Services.Account;

public interface IAccountService
{
    Task<Domain.Models.Account> GetAccountAsync(int id);
    Task<IEnumerable<Domain.Models.Account>> SearchAsync(SearchAccountDto searchAccountDto);
    Task<Domain.Models.Account> RegisterAccountAsync(UpdateAccountDto updateAccountDto);
    Task RemoveAccountAsync(int id);
    Task<Domain.Models.Account> UpdateAccountAsync(int accountId, UpdateAccountDto updateAccountDto);
    Task EnsureEmailBelongsToAccount(int accountId, string email);
    Task<Domain.Models.Account> CrateAccountAsync(AddAccountDto accountDto);
}