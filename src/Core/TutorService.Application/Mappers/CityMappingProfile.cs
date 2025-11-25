using AutoMapper;
using TutorService.Application.DTOs.City;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class CityMappingProfile : Profile
{
    public CityMappingProfile()
    {
        CreateMap<City, CityDto>();
        CreateMap<CityCreateDto, City>();
        CreateMap<CityUpdateDto, City>();
    }
}