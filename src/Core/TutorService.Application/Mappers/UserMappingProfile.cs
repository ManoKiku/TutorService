using TutorService.Application.DTOs.User;
using AutoMapper;
using TutorService.Application.DTOs;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UserUpdateRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified));

        CreateMap<UserDto, User>();
    }
}