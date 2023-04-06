using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Web.Auth;
using ItPlanet.Web.Extensions;
using ItPlanet.Web.Services.Account;
using ItPlanet.Web.Services.Auth;
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
    public async Task<IActionResult> GetAccount([Required] [Range(1, int.MaxValue)] int accountId)
    {
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        try
        {
            var account = await _accountService.GetAccountAsync(accountId);
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
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        var accounts = await _accountService.SearchAsync(searchAccountDto);

        return Ok(accounts);
    }

    [HttpDelete("{accountId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount([Range(1, int.MaxValue)] [Required] int accountId)
    {
        try
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role is Role.Admin)
                return await DeleteAccountByAdmin(accountId).ConfigureAwait(false);
            
            return await DeleteAccountByUserOrChipper(accountId).ConfigureAwait(false);
        }
        catch (AccountRelatedToAnimalException)
        {
            return BadRequest();
        }
    }

    private async Task<IActionResult> DeleteAccountByAdmin(int accountId)
    {
        try
        {
            await _accountService.RemoveAccountAsync(accountId).ConfigureAwait(false);
            return Ok();
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task<IActionResult> DeleteAccountByUserOrChipper(int accountId)
    {
        try
        {
            var (email, _) = Request.ExtractUserData();
            var user = await _accountService.GetAccountAsync(accountId);
            if (user.Email != email)
                return Forbid();

            await _accountService.RemoveAccountAsync(accountId).ConfigureAwait(false);
            return Ok();
        }
        catch (AccountNotFoundException)
        {
            return Forbid();
        }
    }

    [HttpPut("{accountId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateAccount([Required] int accountId, [FromBody] AccountDto accountDto)
    {
        try
        {
            var (email, _) = Request.ExtractUserData();
            await _accountService.EnsureEmailBelongsToAccount(accountId, email);
            var updatedAccount = await _accountService.UpdateAccountAsync(accountId, accountDto);
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

    [HttpPost]
    [Authorize]
    [RoleAuthorize(Role = Role.Admin)]
    public async Task<IActionResult> CreateAccount([FromBody] AddAccountDto accountDto)
    {
        try
        {
            var account = await _accountService.CrateAccountAsync(accountDto).ConfigureAwait(false);
            return Created("", account);
        }
        catch (DuplicateEmailException e)
        {
            return Conflict();
        }
    }
}