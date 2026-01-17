using TutorService.Application.DTOs.Chat;

namespace TutorService.Application.Interfaces;

public interface IChatService
{
    Task<ChatDto> CreateChatAsync(Guid userId, ChatCreateRequest request);
    Task<ChatDto> GetByTutorId(Guid userId, Guid tutorId);
    Task<IEnumerable<ChatDto>> GetUserChatsAsync(Guid userId);
    Task<ChatDto?> GetChatByIdAsync(Guid chatId, Guid userId);
    Task<IEnumerable<MessageDto>> GetChatMessagesAsync(Guid chatId, Guid userId, Guid? beforeMessageId = null);
    Task<int> GetUnreadCountAsync(Guid chatId, Guid userId);
    Task MarkMessagesAsReadAsync(Guid chatId, Guid userId);
}