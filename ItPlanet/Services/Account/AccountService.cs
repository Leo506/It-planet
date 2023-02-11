using ItPlanet.Database;
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

    public async Task<IEnumerable<AccountModel>> SearchAsync(string? firstName, string? lastName, string? email, int from, int size)
    {
        firstName ??= string.Empty;
        lastName ??= string.Empty;
        email ??= string.Empty;
        var searchResult = await _repository.GetByPredicate(x =>
            x.FirstName.ToLower().Contains(firstName.ToLower()) && 
            x.LastName.ToLower().Contains(lastName.ToLower()) && 
            x.Email.ToLower().Contains(email.ToLower()));
        return searchResult.Skip(from).Take(size);
    }
}