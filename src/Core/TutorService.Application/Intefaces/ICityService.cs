using TutorService.Application.DTOs.City;

namespace TutorService.Application.Intefaces;

public interface ICityService
{
    Task<IEnumerable<CityDto>> GetCitiesAsync(string? search, int page, int pageSize);
    Task<CityDto?> GetCityByIdAsync(int id);
    Task<CityDto> CreateCityAsync(CityCreateDto createDto);
    Task<CityDto?> UpdateCityAsync(int id, CityUpdateDto updateDto);
    Task DeleteCityAsync(int id);
}