using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetChatMessagesAsync(Guid chatId, Guid? beforeMessageId = null)
    {
        var query = _dbSet
            .Include(m => m.Sender)
            .Where(m => m.ChatId == chatId && !m.IsDeleted);

        if (beforeMessageId.HasValue)
        {
            var beforeMessage = await _dbSet.FindAsync(beforeMessageId.Value);
            if (beforeMessage != null)
            {
                query = query.Where(m => m.SentAt < beforeMessage.SentAt);
            }
        }

        return await query
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<int> GetChatMessagesCountAsync(Guid chatId)
    {
        return await _dbSet
            .CountAsync(m => m.ChatId == chatId && !m.IsDeleted);
    }

    public async Task<Message?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .Include(m => m.Chat)
            .ThenInclude(c => c!.Tutor)
            .ThenInclude(t => t!.User)
            .Include(m => m.Chat)
            .ThenInclude(c => c!.Student)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> IsMessageSenderAsync(Guid messageId, Guid userId)
    {
        return await _dbSet
            .AnyAsync(m => m.Id == messageId && m.SenderId == userId);
    }
}