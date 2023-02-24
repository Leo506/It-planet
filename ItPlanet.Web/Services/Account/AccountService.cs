using ItPlanet.Domain.Exceptions;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;

namespace ItPlanet.Web.Services.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Domain.Models.Account> GetAccountAsync(int id)
    {
        var account = await _accountRepository.GetAsync(id);
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

    public async Task RemoveAccountAsync(int id)
    {
        var account = await GetAccountAsync(id).ConfigureAwait(false);
        if (account.Animals.Any())
            throw new AccountDeletionException();
        await _accountRepository.DeleteAsync(account).ConfigureAwait(false);
    }

    public async Task<Domain.Models.Account> UpdateAccountAsync(int accountId, AccountDto accountDto)
    {
        if (await _accountRepository.ExistAsync(accountId).ConfigureAwait(false) is false)
            throw new AccountNotFoundException(accountId);

        var accountWithProvidedEmail = await _accountRepository.GetByEmail(accountDto.Email).ConfigureAwait(false);

        if (accountWithProvidedEmail is not null && accountId != accountWithProvidedEmail.Id)
            throw new DuplicateEmailException();

        // TODO Add AutoMapper
        var account = new Domain.Models.Account
        {
            Id = accountId,
            FirstName = accountDto.FirstName,
            LastName = accountDto.LastName,
            Email = accountDto.Email,
            Password = accountDto.Password
        };

        return await _accountRepository.UpdateAsync(account).ConfigureAwait(false);
    }

    public async Task EnsureEmailBelongsToAccount(int accountId, string email)
    {
        var accountByEmail = await _accountRepository.GetByEmail(email).ConfigureAwait(false);
        if (accountByEmail is null)
            return;
        if (accountId != accountByEmail.Id)
            throw new ChangingNotOwnAccountException();
    }
}