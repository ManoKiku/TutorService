using TutorService.Application.DTOs.StudentTutorRelation;

namespace TutorService.Application.Interfaces;

public interface IStudentTutorRelationService
{
    Task<StudentTutorRelationDto> CreateRelationAsync(Guid tutorId, StudentTutorRelationCreateRequest request);
    Task<IEnumerable<StudentTutorRelationDto>> GetMyStudentsAsync(Guid tutorId, string? search = null);
    Task<IEnumerable<StudentTutorRelationDto>> GetMyTutorsAsync(Guid studentId, string? search = null);
    Task<bool> DeleteRelationAsync(Guid tutorId, Guid studentId);
    Task<StudentTutorRelationDto> CheckRelationAsync(Guid studentId, Guid tutorId);
    Task<bool> AreRelatedAsync(Guid studentId, Guid tutorId);
}