using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.ValidationAttributes;

namespace ItPlanet.Domain.Dto;

public class UpdateAccountDto
{
    [Required] [MinLength(1)] public string FirstName { get; set; } = default!;

    [Required] [MinLength(1)] public string LastName { get; set; } = default!;

    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required] [MinLength(1)] public string Password { get; set; } = default!;

    [Required] [Role] public string Role { get; set; } = default!;
}