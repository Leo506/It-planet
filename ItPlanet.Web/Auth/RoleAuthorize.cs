using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ItPlanet.Web.Auth;

public class RoleAuthorize : Attribute, IAsyncAuthorizationFilter
{
    public string Role { get; set; } = default!;
    
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (string.IsNullOrEmpty(Role))
            return Task.CompletedTask;
        
        var principal = context.HttpContext.User;
        var role = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        if (Role == role)
            return Task.CompletedTask;
        
        context.Result = new ForbidResult();
        return Task.CompletedTask;
    }
}