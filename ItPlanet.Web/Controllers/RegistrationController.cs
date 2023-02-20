using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Web.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class RegistrationController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<RegistrationController> _logger;

    public RegistrationController(IAccountService accountService, ILogger<RegistrationController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }


    [HttpPost]
    public async Task<IActionResult> RegisterAccount([FromBody] AccountDto accountDto)
    {
        try
        {
            var account = await _accountService.RegisterAccountAsync(accountDto);
            return Created(string.Empty, account);
        }
        catch (DuplicateEmailException e)
        {
            _logger.LogWarning(e, "Registration failed");
            return Conflict();
        }
    }
}