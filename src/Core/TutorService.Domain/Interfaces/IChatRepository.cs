using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface IChatRepository : IRepository<Chat>
{
    Task<Chat?> GetByIdWithMessagesAsync(Guid id);
    Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId);
    Task<int> GetUserChatsCountAsync(Guid userId);
    Task<Chat?> GetByParticipantsAsync(Guid tutorId, Guid studentId);
    Task<bool> IsUserParticipantAsync(Guid chatId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid chatId, Guid userId);
    Task MarkMessagesAsReadAsync(Guid chatId, Guid userId);
    Task<Chat?> GetByTutorId(Guid studentId, Guid tutorId);
}