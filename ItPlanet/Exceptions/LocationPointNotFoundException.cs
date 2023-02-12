namespace ItPlanet.Exceptions;

public class LocationPointNotFoundException : Exception
{
    public LocationPointNotFoundException(long id) : base($"Location point with id {id} not found")
    {
    }
}