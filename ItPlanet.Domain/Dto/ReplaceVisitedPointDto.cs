using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class ReplaceVisitedPointDto
{
    [Required] [Range(1, long.MaxValue)] public long VisitedLocationPointId { get; set; }

    [Required] [Range(1, long.MaxValue)] public long LocationPointId { get; set; }
}