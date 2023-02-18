using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Dto;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.Animal;
using ItPlanet.Infrastructure.Services.AnimalType;
using ItPlanet.Infrastructure.Services.Auth;
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
}