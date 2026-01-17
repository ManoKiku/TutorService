using TutorService.Application.DTOs.Lesson;
using TutorService.Domain.Enums;

namespace TutorService.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDto> CreateAsync(Guid tutorId, LessonCreateRequest request);
    Task<IEnumerable<LessonDto>> GetLessonsAsync(
        Guid currentUserId,
        string currentUserRole,
        Guid? userId = null,
        LessonStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? tutorId = null,
        Guid? studentId = null);
    Task<LessonDto?> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole);
    Task<LessonDto> UpdateAsync(Guid id, LessonUpdateRequest request, Guid currentUserId, string currentUserRole);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, string currentUserRole);
    Task<IEnumerable<LessonDto>> GetUpcomingLessonsAsync(Guid userId, int daysAhead = 7);
    Task<IEnumerable<LessonDto>> GetCalendarLessonsAsync(Guid userId, int month, int year);
}