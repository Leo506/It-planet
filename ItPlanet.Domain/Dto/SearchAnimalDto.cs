using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class SearchAnimalDto
{
    public DateTime? StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public int? ChipperId { get; set; }

    public long? ChippingLocationId { get; set; }

    public string? LifeStatus { get; set; }

    public string? Gender { get; set; }

    [Range(0, int.MaxValue)] public int From { get; set; } = 0;

    [Range(1, int.MaxValue)] public int Size { get; set; } = 10;
}