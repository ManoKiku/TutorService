using AutoMapper;
using TutorService.Application.DTOs.Subcategory;
using TutorService.Application.DTOs.Tutor;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class SubcategoryMappingProfile : Profile
{
    public SubcategoryMappingProfile()
    {
        CreateMap<Subcategory, SubcategoryDto>();
        CreateMap<SubcategoryDto, Subcategory>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Subjects, opt => opt.Ignore());
    }
}