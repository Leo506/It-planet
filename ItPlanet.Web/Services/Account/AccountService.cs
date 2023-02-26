using AutoMapper;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Repositories.Account;

namespace ItPlanet.Web.Services.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public AccountService(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<Domain.Models.Account> GetAccountAsync(int id)
    {
        var account = await _accountRepository.GetAsync(id);
        return account ?? throw new AccountNotFoundException(id);
    }

    public Task<IEnumerable<Domain.Models.Account>> SearchAsync(SearchAccountDto searchAccountDto)
    {
        // TODO think about searching implementation
        return _accountRepository.FindAsync(searchAccountDto);
    }

    public async Task<Domain.Models.Account> RegisterAccountAsync(AccountDto accountDto)
    {
        if (await _accountRepository.HasAccountWithEmail(accountDto.Email))
            throw new DuplicateEmailException();

        var account = _mapper.Map<Domain.Models.Account>(accountDto);

        return await _accountRepository.CreateAsync(account);
    }

    public async Task RemoveAccountAsync(int id)
    {
        var account = await GetAccountAsync(id).ConfigureAwait(false);
        
        if (AccountRelatedToAnimal())
            throw new AccountDeletionException();
        
        await _accountRepository.DeleteAsync(account).ConfigureAwait(false);

        bool AccountRelatedToAnimal()
        {
            return account.Animals.Any();
        }
    }

    public async Task<Domain.Models.Account> UpdateAccountAsync(int accountId, AccountDto accountDto)
    {
        await EnsureAvailableAccountUpdate(accountId, accountDto);

        var account = _mapper.Map<Domain.Models.Account>(accountDto);
        account.Id = accountId;

        return await _accountRepository.UpdateAsync(account).ConfigureAwait(false);
    }

    private async Task EnsureAvailableAccountUpdate(int accountId, AccountDto accountDto)
    {
        if (await _accountRepository.ExistAsync(accountId).ConfigureAwait(false) is false)
            throw new AccountNotFoundException(accountId);

        if (await IsEmailAlreadyUsed())
            throw new DuplicateEmailException();
        
        async Task<bool> IsEmailAlreadyUsed()
        {
            var accountWithProvidedEmail = await _accountRepository.GetByEmail(accountDto.Email).ConfigureAwait(false);

            return accountWithProvidedEmail is not null && accountId != accountWithProvidedEmail.Id;
        }
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