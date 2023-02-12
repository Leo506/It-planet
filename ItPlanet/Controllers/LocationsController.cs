using System.ComponentModel.DataAnnotations;
using ItPlanet.Exceptions;
using ItPlanet.Services.LocationPoint;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationPointService _locationPointService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILocationPointService locationPointService, ILogger<LocationsController> logger)
    {
        _locationPointService = locationPointService;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetLocationPoint([Range(1, long.MaxValue)] long? id)
    {
        _logger.LogInformation($"Get {nameof(GetLocationPoint)} request");

        if (id is null)
            return BadRequest();

        try
        {
            var point = await _locationPointService.GetLocationPointAsync(id.Value);
            return Ok(point);
        }
        catch (LocationPointNotFoundException e)
        {
            _logger.LogWarning("Location point with id {Id} not found", id);
            return NotFound();
        }
    }
}