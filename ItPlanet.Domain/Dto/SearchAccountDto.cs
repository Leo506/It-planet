using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class SearchAccountDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Range(0, int.MaxValue)] public int From { get; set; } = default;

    [Range(1, int.MaxValue)] public int Size { get; set; } = 10;
}