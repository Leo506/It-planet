using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Dto;
using ItPlanet.Dto;
using ItPlanet.Exceptions;
using ItPlanet.Infrastructure.Services.Animal;
using ItPlanet.Infrastructure.Services.AnimalType;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly IAnimalTypeService _animalTypeService;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(IAnimalService animalService, ILogger<AnimalsController> logger,
        IAnimalTypeService animalTypeService)
    {
        _animalService = animalService;
        _logger = logger;
        _animalTypeService = animalTypeService;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAnimal([Range(1, long.MaxValue)] long? id)
    {
        LogRequest(nameof(GetAnimal));

        if (id is null)
            return BadRequest();

        try
        {
            var animal = await _animalService.GetAnimalAsync(id.Value);
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
        LogRequest(nameof(SearchAnimal));

        var animals = await _animalService.SearchAnimalAsync(searchAnimalDto);

        return Ok(animals);
    }

    [HttpGet("types/{id:long}")]
    public async Task<IActionResult> GetAnimalType([Range(1, long.MaxValue)] long? id)
    {
        LogRequest(nameof(GetAnimalType));

        if (id is null)
            return BadRequest();

        try
        {
            var type = await _animalTypeService.GetAnimalTypeAsync(id.Value);
            return Ok(type);
        }
        catch (AnimalTypeNotFoundException e)
        {
            _logger.LogWarning("Animal type with id {Id} not found", id);
            return NotFound();
        }
    }

    [HttpGet("{animalId:long?}/locations")]
    public async Task<IActionResult> GetVisitedLocations(long? animalId,
        [FromQuery] VisitedLocationDto visitedLocationDto)
    {
        LogRequest(nameof(GetVisitedLocations));

        if (animalId is null)
            return BadRequest();

        try
        {
            var points = await _animalService.GetAnimalVisitedPoints(animalId.Value, visitedLocationDto);
            return Ok(points);
        }
        catch (AnimalNotFoundException e)
        {
            _logger.LogWarning(e, "Failed to find animal");
            return NotFound();
        }
    }

    private void LogRequest(string requestName)
    {
        _logger.LogInformation("Get {RequestName} request", requestName);
    }
}