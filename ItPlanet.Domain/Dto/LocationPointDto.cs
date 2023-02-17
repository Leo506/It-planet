using System.ComponentModel.DataAnnotations;

namespace ItPlanet.Domain.Dto;

public class LocationPointDto
{
    [Required] [Range(-90.0, 90.0)] public double Latitude { get; set; }

    [Required] [Range(-180.0, 180.0)] public double Longitude { get; set; }
}