using TutorService.Domain.Entities;
using TutorService.Domain.Enums;

namespace TutorService.Domain.Interfaces;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetFilteredLessonsAsync(
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null);

    Task<int> GetFilteredLessonsCountAsync(
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null);

    Task<IEnumerable<Lesson>> GetUpcomingLessonsAsync(Guid userId, int daysAhead = 7);
    Task<IEnumerable<Lesson>> GetCalendarLessonsAsync(Guid userId, int month, int year);
    Task<Lesson?> GetByIdWithDetailsAsync(Guid id);
    Task<bool> IsUserParticipantAsync(Guid lessonId, Guid userId);
}