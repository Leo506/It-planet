namespace ItPlanet.Exceptions;

public class AnimalNotFoundException : Exception
{
    public AnimalNotFoundException(long id) : base($"Animal with id {id} not found")
    {
    }
}