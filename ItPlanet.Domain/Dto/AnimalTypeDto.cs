using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class AnimalTypeDto
{
    [Required] public string Type { get; set; } = default!;
}