using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Dto;

public class AccountDto
{
    [Required] [MinLength(1)] public string FirstName { get; set; } = default!;

    [Required] [MinLength(1)] public string LastName { get; set; } = default!;

    [Required] [MinLength(1)] public string Email { get; set; } = default!;

    [Required] [MinLength(1)] public string Password { get; set; } = default!;
}