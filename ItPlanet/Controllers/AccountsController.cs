using ItPlanet.Exceptions;
using ItPlanet.Services.Account;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> GetAccount(int? accountId)
    {
        if (accountId is null or <= 0)
            return BadRequest();

        try
        {
            var account = await _accountService.GetAccountAsync(accountId.Value);
            return Ok(account);
        }
        catch (AccountNotFoundException e)
        {
            return NotFound();
        }
        
    }
}