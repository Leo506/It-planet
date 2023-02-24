using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Models;

namespace ItPlanet.Domain.Dto;

public class AnimalDto
{
    [Required]
    [Range(1f, float.MaxValue)]
    public float Weight { get; set; }
   
    [Required]
    [Range(1f, float.MaxValue)]
    public float Length { get; set; }
    
    [Required]
    [Range(1f, float.MaxValue)]
    public float Height { get; set; }

    [Required] public string Gender { get; set; } = default!;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int ChipperId { get; set; }
    
    public long ChippingLocationId { get; set; }

    [Required] [MinLength(1)] public long[] AnimalTypes { get; set; } = default!;

    public bool IsValid()
    {
        if (GenderConstants.AvailableGenders.Contains(Gender) is false)
            return false;

        if (AnimalTypes.Any(x => x <= 0))
            return false;

        return true;
    }
}