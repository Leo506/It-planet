using System.Text;
using ItPlanet.Infrastructure.Services.Auth;
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
        if (Request.Headers.Authorization.Any() is false)
            return true;

        var headerValue = Request.Headers.Authorization.ToString()["Basic".Length..];
        var decodedBytes = Convert.FromBase64String(headerValue);
        var decodedString = Encoding.UTF8.GetString(decodedBytes);

        var value = decodedString.Split(":");
        var login = value[0];
        var password = value[1];

        return await _authenticationService.TryLogin(login, password);
    }
}