namespace ItPlanet.Domain.Models;

public class LifeStatusConstants
{
    public const string Alive = "ALIVE";

    public const string Dead = "DEAD";

    public static readonly List<string> AvailableLifeStatuses = new() { Alive, Dead };
}