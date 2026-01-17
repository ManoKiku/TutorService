using TutorService.Application.DTOs.Chat;

namespace TutorService.Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, MessageCreateRequest request);
    Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
    Task<MessageDto?> GetMessageByIdAsync(Guid messageId, Guid userId);
}