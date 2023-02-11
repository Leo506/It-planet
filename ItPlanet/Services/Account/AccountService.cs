using ItPlanet.Database;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Services.Account;

public class AccountService : IAccountService
{
    private readonly IRepository<AccountModel, int> _repository;

    public AccountService(IRepository<AccountModel, int> repository)
    {
        _repository = repository;
    }

    public async Task<AccountModel> GetAccountAsync(int id)
    {
        var account = await _repository.GetByIdAsync(id);
        return account ?? throw new AccountNotFoundException(id);
    }

    public async Task<IEnumerable<AccountModel>> SearchAsync(SearchAccountDto searchAccountDto)
    {
        var searchResult = await _repository.GetByPredicate(x =>
            x.FirstName.ToLower().Contains(searchAccountDto.FirstName.ToLower()) && 
            x.LastName.ToLower().Contains(searchAccountDto.LastName.ToLower()) && 
            x.Email.ToLower().Contains(searchAccountDto.Email.ToLower()));
        return searchResult.Skip(searchAccountDto.From).Take(searchAccountDto.Size);
    }
}