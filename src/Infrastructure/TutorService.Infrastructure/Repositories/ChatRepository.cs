using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class ChatRepository : BaseRepository<Chat>, IChatRepository
{
    public ChatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Chat?> GetByIdWithMessagesAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Tutor)
            .ThenInclude(t => t!.User)
            .Include(c => c.Student)
            .Include(c => c.Messages
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                )
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Chat>> GetUserChatsAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.Tutor)
            .ThenInclude(t => t!.User)
            .Include(c => c.Student)
            .Include(c => c.Messages
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .Take(1))
            .Where(c => c.TutorId == userId || c.StudentId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUserChatsCountAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(c => c.TutorId == userId || c.StudentId == userId);
    }

    public async Task<Chat?> GetByParticipantsAsync(Guid tutorId, Guid studentId)
    {
        return await _dbSet
            .Include(c => c.Tutor)
            .ThenInclude(t => t!.User)
            .Include(c => c.Student)
            .FirstOrDefaultAsync(c => c.TutorId == tutorId && c.StudentId == studentId);
    }

    public async Task<bool> IsUserParticipantAsync(Guid chatId, Guid userId)
    {
        return await _dbSet
            .AnyAsync(c => c.Id == chatId && (c.TutorId == userId || c.StudentId == userId));
    }

    public async Task<int> GetUnreadCountAsync(Guid chatId, Guid userId)
    {
        var chat = await _dbSet
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            return 0;

        return chat.Messages
            .Count(m => !m.IsRead && m.SenderId != userId && !m.IsDeleted);
    }

    public async Task MarkMessagesAsReadAsync(Guid chatId, Guid userId)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId && m.SenderId != userId && !m.IsRead && !m.IsDeleted)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Chat?> GetByTutorId(Guid studentId, Guid tutorId)
    {
        return await _dbSet.Where(c => c.StudentId == studentId && c.TutorId == tutorId).FirstOrDefaultAsync();
    }
}