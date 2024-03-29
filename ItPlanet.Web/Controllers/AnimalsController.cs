﻿using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions;
using ItPlanet.Domain.Models;
using ItPlanet.Exceptions;
using ItPlanet.Web.Services.Animal;
using ItPlanet.Web.Services.AnimalType;
using ItPlanet.Web.Services.Auth;
using ItPlanet.Web.Services.VisitedPoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimalsController : PublicControllerBase
{
    private readonly IAnimalService _animalService;
    private readonly IAnimalTypeService _animalTypeService;
    private readonly IVisitedPointsService _visitedPointsService;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(IAnimalService animalService, ILogger<AnimalsController> logger,
        IAnimalTypeService animalTypeService, IHeaderAuthenticationService headerAuthenticationService,
        IVisitedPointsService visitedPointsService) : base(
        headerAuthenticationService)
    {
        _animalService = animalService;
        _logger = logger;
        _animalTypeService = animalTypeService;
        _visitedPointsService = visitedPointsService;
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
        catch (AnimalNotFoundException)
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
        catch (AnimalTypeNotFoundException)
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
            var points = await _visitedPointsService.GetAnimalVisitedPoints(animalId, visitedLocationDto);
            return Ok(points);
        }
        catch (AnimalNotFoundException e)
        {
            _logger.LogWarning(e, "Failed to find animal");
            return NotFound();
        }
    }

    [HttpPost("types")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> CreateAnimalType(AnimalTypeDto dto)
    {
        try
        {
            var newType = await _animalTypeService.CreateTypeAsync(dto);
            return CreatedAtAction(nameof(CreateAnimalType), newType);
        }
        catch (DuplicateAnimalTypeException)
        {
            return Conflict();
        }
    }

    [HttpPut("types/{typeId:long}")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> UpdateAnimalType([Range(1, long.MaxValue)] [Required] long typeId,
        AnimalTypeDto typeDto)
    {
        try
        {
            var updatedType = await _animalTypeService.UpdateType(typeId, typeDto);
            return Ok(updatedType);
        }
        catch (AnimalTypeNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateAnimalTypeException)
        {
            return Conflict();
        }
    }

    [HttpPost("{animalId:long}/types/{typeId:long}")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> AddTypeToAnimal([Required] [Range(1, long.MaxValue)] long animalId,
        [Required] [Range(1, long.MaxValue)] long typeId)
    {
        try
        {
            var type = await _animalTypeService.GetAnimalTypeAsync(typeId);
            var animal = await _animalTypeService.AddTypeToAnimalAsync(animalId, type);
            return CreatedAtAction(nameof(AddTypeToAnimal), animal);
        }
        catch (Exception e) when (e is AnimalNotFoundException or AnimalTypeNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateAnimalTypeException)
        {
            return Conflict();
        }
    }

    [HttpPut("{animalId:long}/types")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> ReplaceAnimalType([Required] [Range(1, long.MaxValue)] long animalId,
        [FromBody] ReplaceAnimalTypeDto replaceDto)
    {
        try
        {
            var oldType = await _animalTypeService.GetAnimalTypeAsync(replaceDto.OldTypeId);
            var newType = await _animalTypeService.GetAnimalTypeAsync(replaceDto.NewTypeId);
            var animal = await _animalTypeService.ReplaceAnimalTypeAsync(animalId, oldType, newType);
            return Ok(animal);
        }
        catch (Exception e) when (e is AnimalNotFoundException or AnimalTypeNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateAnimalTypeException)
        {
            return Conflict();
        }
    }

    [HttpDelete("{animalId:long}/types/{typeId:long}")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> DeleteAnimalTypeFromAnimal([Required] [Range(1, long.MaxValue)] long animalId,
        [Required] [Range(1, long.MaxValue)] long typeId)
    {
        try
        {
            var type = await _animalTypeService.GetAnimalTypeAsync(typeId);
            var animal = await _animalTypeService.DeleteAnimalTypeFromAnimalAsync(animalId, type);
            return Ok(animal);
        }
        catch (Exception e) when (e is AnimalNotFoundException or AnimalTypeNotFoundException)
        {
            return NotFound();
        }
        catch (UnableDeleteAnimalTypeException)
        {
            return BadRequest();
        }
    }

    [HttpDelete("types/{typeId:long}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> DeleteAnimalType([Required] [Range(1, long.MaxValue)] long typeId)
    {
        try
        {
            await _animalTypeService.DeleteTypeAsync(typeId);
            return Ok();
        }
        catch (AnimalTypeDeletionException)
        {
            return BadRequest();
        }
        catch (AnimalTypeNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> CreateAnimal([FromBody] AnimalDto animalDto)
    {
        if (animalDto.IsValid() is false)
            return BadRequest();
        try
        {
            var animal = await _animalService.CreateAnimalAsync(animalDto);
            return CreatedAtAction(nameof(CreateAnimal), animal);
        }
        catch (Exception e) when (e is AnimalTypeNotFoundException or AccountNotFoundException
                                      or LocationPointNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateAnimalTypeException)
        {
            return Conflict();
        }
    }

    [HttpPost("{animalId:long}/locations/{pointId:long}")]
    [Authorize]
    public async Task<IActionResult> AddVisitedPoint([Required] [Range(1, long.MaxValue)] long animalId,
        [Required] [Range(1, long.MaxValue)] long pointId)
    {
        try
        {
            var visitedPoint = await _visitedPointsService.AddVisitedPointAsync(animalId, pointId).ConfigureAwait(false);

            return CreatedAtAction(nameof(AddVisitedPoint), visitedPoint);
        }
        catch (UnableAddPointException)
        {
            return BadRequest();
        }
        catch (Exception e) when (e is AnimalNotFoundException or LocationPointNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{animalId:long}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> DeleteAnimal([Required] [Range(1, long.MaxValue)] long animalId)
    {
        try
        {
            await _animalService.DeleteAnimalAsync(animalId);
            return Ok();
        }
        catch (UnableDeleteAnimalException)
        {
            return BadRequest();
        }
        catch (AnimalNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{animalId:long}")]
    [Authorize(Roles = Role.AdminOrChipper)]
    public async Task<IActionResult> UpdateAnimal([Required] [Range(1, long.MaxValue)] long animalId,
        [FromBody] UpdateAnimalDto updateDto)
    {
        if (updateDto.IsValid() is false)
            return BadRequest();

        try
        {
            var animal = await _animalService.UpdateAnimalAsync(animalId, updateDto);
            return Ok(animal);
        }
        catch (Exception e) when (e is AnimalNotFoundException or AccountNotFoundException
                                      or LocationPointNotFoundException)
        {
            return NotFound();
        }
        catch (UnableUpdateAnimalException)
        {
            return BadRequest();
        }
    }

    [HttpPut("{animalId:long}/locations")]
    [Authorize]
    public async Task<IActionResult> ReplaceVisitedPoint([Required] [Range(1, long.MaxValue)] long animalId,
        [FromBody] ReplaceVisitedPointDto replaceDto)
    {
        try
        {
            var visitedPoint = await _visitedPointsService.UpdateVisitedPoint(animalId, replaceDto);
            return Ok(visitedPoint);
        }
        catch (Exception e) when (e is AnimalNotFoundException or LocationPointNotFoundException)
        {
            return NotFound();
        }
        catch (UnableChangeVisitedPoint e)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{animalId:long}/locations/{visitedPointId:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteVisitedPoint([Required] [Range(1, long.MaxValue)] long animalId,
        [Required] [Range(1, long.MaxValue)] long visitedPointId)
    {
        try
        {
            await _visitedPointsService.DeleteVisitedPointAsync(animalId, visitedPointId);
            return Ok();
        }
        catch (Exception e) when (e is AnimalNotFoundException or VisitedPointNotFoundException)
        {
            return NotFound();
        }
    }
}