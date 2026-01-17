using AutoMapper;
using TutorService.Application.DTOs.Chat;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<Chat, ChatDto>()
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => 
                s.Tutor != null && s.Tutor.User != null 
                    ? $"{s.Tutor.User.FirstName} {s.Tutor.User.LastName}" 
                    : string.Empty))
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => 
                s.Student != null 
                    ? $"{s.Student.FirstName} {s.Student.LastName}" 
                    : string.Empty));

        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderName, opt => opt.MapFrom(s => 
                s.Sender != null 
                    ? $"{s.Sender.FirstName} {s.Sender.LastName}" 
                    : string.Empty));
    }
}