using System.Text;

namespace ItPlanet.Web.Extensions;

public static class RequestExtensions
{
    public static (string, string) ExtractUserData(this HttpRequest request)
    {
        if (request.Headers.Authorization.Any() is false)
            return ("", "");
        
        var headerValue = request.Headers.Authorization.ToString()["Basic".Length..];
        var decodedBytes = Convert.FromBase64String(headerValue);
        var decodedString = Encoding.UTF8.GetString(decodedBytes);

        var value = decodedString.Split(":");
        var email = value[0];
        var password = value[1];

        return (email, password);
    }
}