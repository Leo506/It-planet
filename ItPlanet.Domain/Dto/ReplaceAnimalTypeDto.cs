using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class ReplaceAnimalTypeDto
{
    [Required] [Range(1, long.MaxValue)] public long OldTypeId { get; set; }

    [Required] [Range(1, long.MaxValue)] public long NewTypeId { get; set; }
}