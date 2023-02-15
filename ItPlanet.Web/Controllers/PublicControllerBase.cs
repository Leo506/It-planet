using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

public class PublicControllerBase : ControllerBase
{
    private readonly IHeaderAuthenticationService _authenticationService;

    public PublicControllerBase(IHeaderAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    protected async Task<bool> AllowedToHandleRequest()
    {
        var (login, password) = Request.ExtractUserData();
        if (string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password))
            return true;
        return await _authenticationService.TryLogin(login, password);
    }
}