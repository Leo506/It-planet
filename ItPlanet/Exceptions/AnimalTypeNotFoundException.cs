namespace ItPlanet.Exceptions;

public class AnimalTypeNotFoundException : Exception
{
    public AnimalTypeNotFoundException(long id) : base($"Animal type with id {id} not found")
    {
    }
}