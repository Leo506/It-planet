using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Exceptions;
using ItPlanet.Web.Services.Auth;
using ItPlanet.Web.Services.LocationPoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGeoHash;

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
        catch (UnableDeleteLocationPointException e)
        {
            return BadRequest();
        }
    }

    [HttpPut("{pointId:long}")]
    [Authorize]
    public async Task<IActionResult> UpdateLocationPoint([Range(1, long.MaxValue)] [Required] long pointId,
        LocationPointDto pointDto)
    {
        try
        {
            var result = await _locationPointService.UpdatePointAsync(pointId, pointDto);

            return Ok(result);
        }
        catch (LocationPointNotFoundException e)
        {
            return NotFound();
        }
        catch (DuplicateLocationPointException e)
        {
            return Conflict();
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetLocationId([FromQuery] double latitude, [FromQuery] double longitude)
    {
        try
        {
            var id = await _locationPointService.GetLocationPointIdAsync(latitude, longitude).ConfigureAwait(false);
            return Ok(id);
        }
        catch (LocationPointNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("geohash")]
    [Authorize]
    public async Task<IActionResult> Test2([FromQuery] double latitude, [FromQuery] double longitude)
    {
        return Ok(GeoHash.Encode(latitude, longitude, 12));
    }
    
    [HttpGet("geohashv2")]
    [Authorize]
    public async Task<IActionResult> Test2V2([FromQuery] double latitude, [FromQuery] double longitude)
    {
        var hashCode = GeoHash.Encode(latitude, longitude, 12);
        return Ok(Convert.ToBase64String(Encoding.UTF8.GetBytes(hashCode)));
    }
    
    [HttpGet("geohashv3")]
    [Authorize]
    public async Task<IActionResult> Test2V3([FromQuery] double latitude, [FromQuery] double longitude)
    {
        var hashCode = GeoHash.Encode(latitude, longitude, 12);
        var md5HashCode = MD5.HashData(Encoding.UTF8.GetBytes(hashCode));
        for (var i = 0; i < md5HashCode.Length / 2; i++)
        {
            (md5HashCode[i], md5HashCode[md5HashCode.Length - i - 1]) =
                (md5HashCode[md5HashCode.Length - i - 1], md5HashCode[i]);
        }

        var result = Convert.ToBase64String(md5HashCode);
        return Ok(result);
    }
}