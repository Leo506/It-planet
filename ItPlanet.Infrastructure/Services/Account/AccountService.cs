using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;

namespace ItPlanet.Infrastructure.Services.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Domain.Models.Account> GetAccountAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account ?? throw new AccountNotFoundException(id);
    }

    public Task<IEnumerable<Domain.Models.Account>> SearchAsync(SearchAccountDto searchAccountDto)
    {
        return _accountRepository.FindAsync(searchAccountDto);
    }

    public async Task<Domain.Models.Account> RegisterAccountAsync(AccountDto accountDto)
    {
        if (await _accountRepository.HasAccountWithEmail(accountDto.Email))
            throw new DuplicateEmailException();

        // TODO use AutoMapper
        var account = new Domain.Models.Account
        {
            FirstName = accountDto.FirstName,
            LastName = accountDto.LastName,
            Email = accountDto.Email,
            Password = accountDto.Password
        };

        return await _accountRepository.CreateAsync(account);
    }
}