using AutoMapper;
using TutorService.Application.DTOs.Category;
using TutorService.Application.DTOs.Subject;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class SubjectMappingProfile : Profile
{
    public SubjectMappingProfile()
    {
        CreateMap<Subject, SubjectDto>();
        CreateMap<SubjectDto, Subject>()
            .ForMember(dest => dest.Subcategory, opt => opt.Ignore())
            .ForMember(dest => dest.TutorPosts, opt => opt.Ignore());
    }
}