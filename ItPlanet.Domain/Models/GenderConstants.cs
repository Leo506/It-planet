namespace ItPlanet.Domain.Models;

public class GenderConstants
{
    public const string Male = "MALE";

    public const string Female = "FEMALE";

    public const string Other = "OTHER";

    public static readonly IReadOnlyList<string> AvailableGenders = new List<string>() { Male, Female, Other };
}