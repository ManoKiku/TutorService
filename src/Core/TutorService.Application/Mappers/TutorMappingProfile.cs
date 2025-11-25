using AutoMapper;
using TutorService.Application.DTOs.Tutor;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class TutorMappingProfile : Profile
{
    public TutorMappingProfile()
    {
        CreateMap<TutorProfile, TutorProfileDto>();
        CreateMap<TutorProfileDto, TutorProfile>();
        CreateMap<TutorPost, TutorPostDto>()
            .ForMember(d => d.SubjectName, opt => opt.MapFrom(s => s.Subject.Name))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Tutor.User.FirstName + " " + s.Tutor.User.LastName))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.TutorPostTags.Select(t => t.Tag)));
        CreateMap<TutorPostDto, TutorPost>();
        CreateMap<Tag, TagDto>();
    }
}