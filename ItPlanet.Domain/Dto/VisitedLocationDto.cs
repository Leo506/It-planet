using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class VisitedLocationDto
{
    public DateTime? StarDateTime { get; set; }
    
    public DateTime? EndDateTime { get; set; }

    [Range(0, int.MaxValue)]
    public int From { get; set; } = 0;

    [Range(1, int.MaxValue)]
    public int Size { get; set; } = 10;
}