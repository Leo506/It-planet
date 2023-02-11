namespace ItPlanet.Dto;

public class SearchAccountDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int From { get; set; } = default;

    public int Size { get; set; } = 10;
}
