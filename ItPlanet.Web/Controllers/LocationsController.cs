using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Infrastructure.Services.LocationPoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController : PublicControllerBase
{
    private readonly ILocationPointService _locationPointService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILocationPointService locationPointService, ILogger<LocationsController> logger,
        IHeaderAuthenticationService headerAuthenticationService) : base(headerAuthenticationService)
    {
        _locationPointService = locationPointService;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetLocationPoint([Range(1, long.MaxValue)] [Required] long id)
    {
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        try
        {
            var point = await _locationPointService.GetLocationPointAsync(id);
            return Ok(point);
        }
        catch (LocationPointNotFoundException e)
        {
            _logger.LogWarning("Location point with id {Id} not found", id);
            return NotFound();
        }
    }

    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> CreateLocationPoint([FromBody] LocationPointDto dto)
    {
        try
        {
            var point = await _locationPointService.CreatePointAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(CreateLocationPoint), point);
        }
        catch (DuplicateLocationPointException e)
        {
            return Conflict();
        }
    }

    [HttpDelete("{pointId:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteLocationPoint([Range(1, long.MaxValue)] [Required] long pointId)
    {
        try
        {
            await _locationPointService.DeletePointAsync(pointId);
            return Ok();
        }
        catch (LocationPointNotFoundException e)
        {
            return NotFound();
        }
    }
}