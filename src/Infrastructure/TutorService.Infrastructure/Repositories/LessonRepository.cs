using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class LessonRepository : BaseRepository<Lesson>, ILessonRepository
{
    public LessonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Lesson>> GetFilteredLessonsAsync(
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null)
    {
        var query = _context.Lessons
            .Include(l => l.Tutor)
            .ThenInclude(t => t!.User)
            .Include(l => l.Student)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(l => l.Tutor!.User!.Id == userId || l.StudentId == userId);
        }

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(l => l.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(l => l.EndTime <= endDate.Value);
        }

        if (tutorId.HasValue)
        {
            query = query.Where(l => l.TutorId == tutorId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(l => l.StudentId == studentId.Value);
        }

        return await query
            .OrderBy(l => l.StartTime)
            .ToListAsync();
    }

    public async Task<int> GetFilteredLessonsCountAsync(
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null)
    {
        var query = _context.Lessons.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(l => l.TutorId == userId || l.StudentId == userId);
        }

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(l => l.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(l => l.EndTime <= endDate.Value);
        }

        if (tutorId.HasValue)
        {
            query = query.Where(l => l.TutorId == tutorId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(l => l.StudentId == studentId.Value);
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Lesson>> GetUpcomingLessonsAsync(Guid userId, int daysAhead = 7)
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(daysAhead);

        return await _context.Lessons
            .Include(l => l.Tutor)
            .ThenInclude(t => t!.User)
            .Include(l => l.Student)
            .Where(l => (l.TutorId == userId || l.StudentId == userId) &&
                       l.StartTime >= startDate &&
                       l.StartTime <= endDate &&
                       l.Status == LessonStatus.Scheduled)
            .OrderBy(l => l.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lesson>> GetCalendarLessonsAsync(Guid userId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return await _context.Lessons
            .Include(l => l.Tutor)
            .ThenInclude(t => t!.User)
            .Include(l => l.Student)
            .Where(l => (l.TutorId == userId || l.StudentId == userId) &&
                       l.StartTime >= startDate &&
                       l.StartTime <= endDate)
            .OrderBy(l => l.StartTime)
            .ToListAsync();
    }

    public override async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons
            .Include(l => l.Tutor)
            .ThenInclude(t => t!.User)
            .Include(l => l.Student)
            .Include(l => l.Assignments)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Lesson?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Lessons
            .Include(l => l.Tutor)
            .ThenInclude(t => t!.User)
            .Include(l => l.Student)
            .Include(l => l.Assignments)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
    public async Task<bool> IsUserParticipantAsync(Guid lessonId, Guid userId)
    {
        return await _context.Lessons
            .AnyAsync(l => l.Id == lessonId && (l.TutorId == userId || l.StudentId == userId));
    }
}