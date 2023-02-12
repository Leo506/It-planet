using System.Buffers.Text;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ItPlanet.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ItPlanet.Web.Auth;

public class HeaderAuthenticationHandler : AuthenticationHandler<HeaderAuthenticationOptions>
{
    private readonly IHeaderAuthenticationService _authenticationService;
    
    public HeaderAuthenticationHandler(IOptionsMonitor<HeaderAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHeaderAuthenticationService authenticationService) : base(options, logger, encoder, clock)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        
        if (Request.Headers.Authorization.Any() is false)
            return AuthenticateResult.Fail("Unauthorized");

        var headerValue = Request.Headers.Authorization.ToString()["Basic".Length..];
        var decodedBytes = Convert.FromBase64String(headerValue);
        var decodedString = Encoding.UTF8.GetString(decodedBytes);

        var value = decodedString.Split(":");
        var login = value[0];
        var password = value[1];
        
        if (await _authenticationService.TryLogin(login, password))
        {
            var identity = new ClaimsIdentity(new List<Claim>(), Scheme.Name);
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), "Header"));
        }
        
        return AuthenticateResult.Fail("Unauthorized");
    }
}