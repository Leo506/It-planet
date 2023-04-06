using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class RegisterAccountDto
{
    [Required] [MinLength(1)] public string FirstName { get; set; } = default!;

    [Required] [MinLength(1)] public string LastName { get; set; } = default!;

    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required] [MinLength(1)] public string Password { get; set; } = default!;
}