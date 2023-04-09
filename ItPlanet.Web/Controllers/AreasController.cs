using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Exceptions.Areas;
using ItPlanet.Domain.Models;
using ItPlanet.Web.Auth;
using ItPlanet.Web.Services.Area;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace ItPlanet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AreasController : ControllerBase
{
    private readonly IAreaService _areaService;
    private readonly IMapper _mapper;

    public AreasController(IAreaService areaService, IMapper mapper)
    {
        _areaService = areaService;
        _mapper = mapper;
    }

    [HttpPost("")]
    [Authorize]
    [RoleAuthorize(Role = Role.Admin)]
    public async Task<IActionResult> CreateArea([FromBody] CreateAreaDto createAreaDto)
    {
        if (createAreaDto.IsValidArea() is false)
            return BadRequest();

        try
        {
            var areaModel = _mapper.Map<Area>(createAreaDto);
            var area = await _areaService.CreateAreaAsync(areaModel).ConfigureAwait(false);
            return Created("", area);
        }
        catch (InvalidAreaPointsDataException e)
        {
            return BadRequest(e.GetType().Name);
        }
        catch (ConflictWithExistingAreasException)
        {
            return Conflict();
        }
    }

    [HttpGet("{areaId:long}")]
    [Authorize]
    public async Task<IActionResult> GetArea([Required] [Range(1, long.MaxValue)] long areaId)
    {
        try
        {
            var area = await _areaService.GetAreaById(areaId);
            return Ok(area);
        }
        catch (AreaNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{areaId:long}")]
    [Authorize]
    [RoleAuthorize(Role = Role.Admin)]
    public async Task<IActionResult> DeleteArea([Required] [Range(1, long.MaxValue)] long areaId)
    {
        try
        {
            await _areaService.DeleteAreaById(areaId).ConfigureAwait(false);
            return Ok();
        }
        catch (AreaNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{areaId:long}")]
    [Authorize]
    [RoleAuthorize(Role = Role.Admin)]
    public async Task<IActionResult> UpdateArea([Required] [Range(1, long.MaxValue)] long areaId,
        [FromBody] CreateAreaDto areaDto)
    {
        if (areaDto.IsValidArea() is false)
            return BadRequest();

        try
        {
            var areaModel = _mapper.Map<Area>(areaDto);
            var area = await _areaService.UpdateArea(areaId, areaModel).ConfigureAwait(false);
            return Ok(area);
        }
        catch (InvalidAreaPointsDataException e)
        {
            return BadRequest(e.GetType().Name);
        }
        catch (ConflictWithExistingAreasException)
        {
            return Conflict();
        }
    }
}