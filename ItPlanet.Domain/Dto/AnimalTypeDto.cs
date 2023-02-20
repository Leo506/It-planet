using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class AnimalTypeDto
{
    [Required(AllowEmptyStrings = false)] public string Type { get; set; } = default!;
}