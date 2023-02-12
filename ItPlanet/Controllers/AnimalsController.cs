using System.ComponentModel.DataAnnotations;
using ItPlanet.Exceptions;
using ItPlanet.Services.Animal;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(IAnimalService animalService, ILogger<AnimalsController> logger)
    {
        _animalService = animalService;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAnimal([Range(1, long.MaxValue)] long? id)
    {
        _logger.LogInformation($"Get {nameof(GetAnimal)} request");

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
}