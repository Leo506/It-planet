using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Services.Account;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Controllers;

[ApiController]
[Route("[controller]")]
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
        _logger.LogInformation($"Get {nameof(RegisterAccount)} request");

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