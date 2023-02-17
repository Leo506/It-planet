using System.Text;

namespace ItPlanet.UnitTests.ControllersTests.Helpers;

public class AuthHeaderHelper
{
    public static string GetAuthorizationHeaderValue(string email = "test@test.com", string password = "password")
    {
        var encodedHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));
        var headerValue = $"Basic {encodedHeaderValue}";
        return headerValue;
    }
}