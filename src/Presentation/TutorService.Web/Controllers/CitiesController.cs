using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.City;
using TutorService.Application.Intefaces;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var cities = await _cityService.GetCitiesAsync(search, page, pageSize);
        return Ok(cities);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CityDto>> CreateCity(CityCreateDto createDto)
    {
        var cityDto = await _cityService.CreateCityAsync(createDto);
        return CreatedAtAction(nameof(GetCities), new { id = cityDto.Id }, cityDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CityDto>> UpdateCity(int id, CityUpdateDto updateDto)
    {
        var cityDto = await _cityService.UpdateCityAsync(id, updateDto);
        return Ok(cityDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        await _cityService.DeleteCityAsync(id);

        return NoContent();
    }
}