using AutoMapper;
using TutorService.Application.DTOs.City;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class CityService : ICityService
{
    private readonly ICrudRepository<City>  _cityRepository;
    private readonly IMapper _mapper;

    public CityService(ICrudRepository<City> cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CityDto>> GetCitiesAsync()
    {
        var cities = await _cityRepository.GetAllAsync();
        
        return _mapper.Map<IEnumerable<CityDto>>(cities);
    }

    public async Task<CityDto?> GetCityByIdAsync(int id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        return _mapper.Map<CityDto?>(city);
    }

    public async Task<CityDto> CreateCityAsync(CityCreateDto createDto)
    {
        var city = _mapper.Map<Domain.Entities.City>(createDto);
        var createdCity = await _cityRepository.CreateAsync(city);
        return _mapper.Map<CityDto>(createdCity);
    }

    public async Task<CityDto?> UpdateCityAsync(int id, CityUpdateDto updateDto)
    {
        var existingCity = await _cityRepository.GetByIdAsync(id);
        if (existingCity == null)
            throw new KeyNotFoundException($"City with ID {id} not found");

        _mapper.Map(updateDto, existingCity);
        var updatedCity = await _cityRepository.UpdateAsync(existingCity);
        return _mapper.Map<CityDto>(updatedCity);
    }

    public async Task DeleteCityAsync(int id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city == null) 
            throw new KeyNotFoundException($"City with ID {id} not found");

        await _cityRepository.DeleteAsync(city);
    }
}