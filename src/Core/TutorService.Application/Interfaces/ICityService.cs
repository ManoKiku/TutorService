using TutorService.Application.DTOs.City;

namespace TutorService.Application.Interfaces;

public interface ICityService
{
    Task<IEnumerable<CityDto>> GetCitiesAsync();
    Task<CityDto?> GetCityByIdAsync(int id);
    Task<CityDto> CreateCityAsync(CityCreateDto createDto);
    Task<CityDto?> UpdateCityAsync(int id, CityUpdateDto updateDto);
    Task DeleteCityAsync(int id);
}