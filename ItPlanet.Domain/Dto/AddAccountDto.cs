using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.ValidationAttributes;

namespace ItPlanet.Domain.Dto;

public class AddAccountDto
{
    [Required(AllowEmptyStrings = false)]
    [MinLength(1)]
    public string FirstName { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    [MinLength(1)]
    public string LastName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [Role]
    public string Role { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = default!;
}