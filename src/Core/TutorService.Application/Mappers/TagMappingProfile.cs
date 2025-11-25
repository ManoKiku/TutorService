using AutoMapper;
using TutorService.Application.DTOs.Tag;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>();
        CreateMap<TagCreateDto, Tag>();
        CreateMap<TagUpdateDto, Tag>();
    }
}