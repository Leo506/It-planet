using System.Security.Claims;
using System.Text.Encodings.Web;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ItPlanet.Web.Auth;

public class HeaderAuthenticationHandler : AuthenticationHandler<HeaderAuthenticationOptions>
{
    private readonly IHeaderAuthenticationService _authenticationService;

    public HeaderAuthenticationHandler(IOptionsMonitor<HeaderAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, IHeaderAuthenticationService authenticationService) : base(options,
        logger, encoder, clock)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var (login, password) = Request.ExtractUserData();

        if (await _authenticationService.TryLogin(login, password))
        {
            var identity = new ClaimsIdentity(new List<Claim>(), Scheme.Name);
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), "Header"));
        }

        return AuthenticateResult.Fail("Unauthorized");
    }
}