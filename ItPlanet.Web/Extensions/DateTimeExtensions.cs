namespace ItPlanet.Web.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimSeconds(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
    }
}