using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Web.Services.Animal;
using ItPlanet.Web.Services.AnimalType;
using ItPlanet.Web.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimalsController : PublicControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly IAnimalTypeService _animalTypeService;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(IAnimalService animalService, ILogger<AnimalsController> logger,
        IAnimalTypeService animalTypeService, IHeaderAuthenticationService headerAuthenticationService) : base(
        headerAuthenticationService)
    {
        _animalService = animalService;
        _logger = logger;
        _animalTypeService = animalTypeService;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAnimal([Range(1, long.MaxValue)] [Required] long id)
    {
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        try
        {
            var animal = await _animalService.GetAnimalAsync(id);
            return Ok(animal);
        }
        catch (AnimalNotFoundException e)
        {
            _logger.LogWarning("Animal with {Id} not found", id);
            return NotFound();
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAnimal([FromQuery] SearchAnimalDto searchAnimalDto)
    {
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        var animals = await _animalService.SearchAnimalAsync(searchAnimalDto);

        return Ok(animals);
    }

    [HttpGet("types/{id:long}")]
    public async Task<IActionResult> GetAnimalType([Range(1, long.MaxValue)] [Required] long id)
    {
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        try
        {
            var type = await _animalTypeService.GetAnimalTypeAsync(id);
            return Ok(type);
        }
        catch (AnimalTypeNotFoundException e)
        {
            _logger.LogWarning("Animal type with id {Id} not found", id);
            return NotFound();
        }
    }

    [HttpGet("{animalId:long?}/locations")]
    public async Task<IActionResult> GetVisitedLocations([Required] long animalId,
        [FromQuery] VisitedLocationDto visitedLocationDto)
    {
        if (await AllowedToHandleRequest() is false)
            return Unauthorized();

        try
        {
            var points = await _animalService.GetAnimalVisitedPoints(animalId, visitedLocationDto);
            return Ok(points);
        }
        catch (AnimalNotFoundException e)
        {
            _logger.LogWarning(e, "Failed to find animal");
            return NotFound();
        }
    }

    [HttpPost("types")]
    [Authorize]
    public async Task<IActionResult> CreateAnimalType(AnimalTypeDto dto)
    {
        try
        {
            var newType = await _animalTypeService.CreateTypeAsync(dto);
            return CreatedAtAction(nameof(CreateAnimalType), newType);
        }
        catch (DuplicateAnimalTypeException e)
        {
            return Conflict();
        }
    }

    [HttpPut("types/{typeId:long}")]
    [Authorize]
    public async Task<IActionResult> UpdateAnimalType([Range(1, long.MaxValue)] [Required] long typeId,
        AnimalTypeDto typeDto)
    {
        try
        {
            var updatedType = await _animalTypeService.UpdateType(typeId, typeDto);
            return Ok(updatedType);
        }
        catch (AnimalTypeNotFoundException e)
        {
            return NotFound();
        }
        catch (DuplicateAnimalTypeException e)
        {
            return Conflict();
        }
    }
    
    [HttpDelete("types/{typeId:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteAnimalType([Required] [Range(1, long.MaxValue)] long typeId)
    {
        try
        {
            await _animalTypeService.DeleteTypeAsync(typeId);
            return Ok();
        }
        catch (AnimalTypeDeletionException e)
        {
            return BadRequest();
        }
        catch (AnimalTypeNotFoundException e)
        {
            return NotFound();
        }
    }
}