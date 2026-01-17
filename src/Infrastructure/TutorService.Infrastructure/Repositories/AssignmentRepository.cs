using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class AssignmentRepository : BaseRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Assignment>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _dbSet
            .Include(a => a.Lesson)
            .ThenInclude(l => l!.Tutor)
            .ThenInclude(t => t!.User)
            .Include(a => a.Lesson)
            .ThenInclude(l => l!.Student)
            .Where(a => a.LessonId == lessonId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Assignment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Lesson)
            .ThenInclude(l => l!.Tutor)
            .ThenInclude(t => t!.User)
            .Include(a => a.Lesson)
            .ThenInclude(l => l!.Student)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> IsUserParticipantAsync(Guid id, Guid userId)
    {
        return await _dbSet
            .Include(a => a.Lesson)
            .AnyAsync(a => a.Id == id && 
                           (a.Lesson!.TutorId == userId || a.Lesson!.StudentId == userId));
    }
}