using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.Account;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : PublicControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger,
        IHeaderAuthenticationService headerAuthenticationService) : base(headerAuthenticationService)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> GetAccount(int? accountId)
    {
        _logger.LogInformation($"Get {nameof(GetAccount)} request");

        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        if (accountId is null or <= 0)
            return BadRequest();

        try
        {
            var account = await _accountService.GetAccountAsync(accountId.Value);
            _logger.LogInformation("Account with id {AccountId} successfully found", accountId);
            return Ok(account);
        }
        catch (AccountNotFoundException e)
        {
            _logger.LogWarning(e, "Account was not found");
            return NotFound();
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAccounts([FromQuery] SearchAccountDto searchAccountDto)
    {
        _logger.LogInformation($"Get {nameof(SearchAccounts)} request");

        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        var accounts = await _accountService.SearchAsync(searchAccountDto);

        return Ok(accounts);
    }

    [HttpDelete("{accountId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount([Range(1, int.MaxValue)] int? accountId)
    {
        if (accountId is null)
            return BadRequest();

        try
        {
            await _accountService.RemoveAccountAsync(accountId.Value).ConfigureAwait(false);
            return Ok();
        }
        catch (AccountDeletionException e)
        {
            return BadRequest();
        }
        catch (AccountNotFoundException e)
        {
            return Forbid();
        }
    }

    [HttpPut("{accountId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateAccount(int? accountId, [FromBody] AccountDto accountDto)
    {
        if (accountId is null)
            return BadRequest();

        try
        {
            var (email, _) = Request.ExtractUserData();
            await _accountService.EnsureEmailBelongsToAccount(accountId.Value, email);
            var updatedAccount = await _accountService.UpdateAccountAsync(accountId.Value, accountDto);
            return Ok(updatedAccount);
        }
        catch (AccountNotFoundException e)
        {
            return Forbid();
        }
        catch (ChangingNotOwnAccountException e)
        {
            return Forbid();
        }
        catch (DuplicateEmailException e)
        {
            return Conflict();
        }
    }
}