using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Models;

namespace ItPlanet.Domain.Dto;

public class UpdateAnimalDto
{
    [Required]
    [Range(float.Epsilon, float.MaxValue)]
    public float Weight { get; set; }

    [Required]
    [Range(float.Epsilon, float.MaxValue)]
    public float Length { get; set; }

    [Required]
    [Range(float.Epsilon, float.MaxValue)]
    public float Height { get; set; }

    [Required] public string Gender { get; set; } = default!;

    [Required] [Range(1, int.MaxValue)] public int ChipperId { get; set; }

    [Required]
    [Range(typeof(long), "1", "9223372036854775807")]
    public long ChippingLocationId { get; set; }

    [Required] public string LifeStatus { get; set; } = default!;

    public bool IsValid()
    {
        if (GenderConstants.AvailableGenders.Contains(Gender) is false)
            return false;

        if (LifeStatusConstants.AvailableLifeStatuses.Contains(LifeStatus) is false)
            return false;

        return true;
    }
}