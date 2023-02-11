using ItPlanet.Dto;
using ItPlanet.Models;

namespace ItPlanet.Services.Account;

public interface IAccountService
{
    Task<AccountModel> GetAccountAsync(int id);
    Task<IEnumerable<AccountModel>> SearchAsync(SearchAccountDto searchAccountDto);
}