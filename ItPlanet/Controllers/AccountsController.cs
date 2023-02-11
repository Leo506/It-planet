﻿using ItPlanet.Exceptions;
using ItPlanet.Services.Account;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> GetAccount(int? accountId)
    {
        _logger.LogInformation($"Get {nameof(GetAccount)} request");
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
    public async Task<IActionResult> SearchAccounts(string? firstName = default!, string? lastName = default!,
        string? email = default!, int from = 0, int size = 10)
    {
        _logger.LogInformation($"Get {nameof(SearchAccounts)} request");

        var accounts = await _accountService.SearchAsync(firstName, lastName, email, from, size);

        return Ok(accounts);
    }
}