using ItPlanet.Web.Middlewares;

namespace ItPlanet.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        var logger = builder.ApplicationServices.GetRequiredService<ILogger<RequestLoggingMiddleware>>();
        return builder.UseMiddleware<RequestLoggingMiddleware>(logger);
    }
}