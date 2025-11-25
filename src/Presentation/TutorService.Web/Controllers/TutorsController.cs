using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutorService.Application.DTOs.Tutor;
using TutorService.Application.Intefaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutorsController : ControllerBase
{
    private readonly ITutorProfileService _tutorService;

    public TutorsController(ITutorProfileService tutorService)
    {
        _tutorService = tutorService;
    }

    [HttpPut("profile")]
    [Authorize(Roles = "Tutor")]
    public async Task<ActionResult<TutorProfileDto>> UpsertProfile([FromBody] TutorProfileUpdateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        
        var dto = await _tutorService.UpsertAsync(userId, request);
        
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TutorProfileDto>> GetById(Guid id)
    {
        var dto = await _tutorService.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<object>> Search([
        FromQuery] int? categoryId,
        [FromQuery] int? subcategoryId,
        [FromQuery] int? subjectId,
        [FromQuery] int? cityId,
        [FromQuery] string? tagIds,
        [FromQuery] decimal? minRate,
        [FromQuery] decimal? maxRate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        IEnumerable<int>? tags = null;
        if (!string.IsNullOrWhiteSpace(tagIds))
        {
            tags = tagIds.Split(',').Select(s => int.Parse(s.Trim()));
        }

        var (results, total) = await _tutorService.SearchAsync(categoryId, subcategoryId, subjectId, cityId, tags, minRate, maxRate, page, pageSize, search);
        return Ok(new { results, total });
    }

    [HttpPost("cities")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> AddCity([FromBody] CityRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        await _tutorService.AddCityAsync(userId, request.CityId);
        return NoContent();
    }

    [HttpDelete("cities/{cityId}")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> RemoveCity(int cityId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        await _tutorService.RemoveCityAsync(userId, cityId);
        return NoContent();
    }

    [HttpGet("{id}/cities")]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities(Guid id)
    {
        var cities = await _tutorService.GetCitiesAsync(id);
        return Ok(cities);
    }
}
