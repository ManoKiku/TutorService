using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Interfaces;

public interface IStudentTutorRelationRepository : ICrudRepository<StudentTutorRelation>
{
    Task<bool> RelationExistsAsync(Guid studentId, Guid tutorId);
    Task<StudentTutorRelation?> GetByStudentAndTutorAsync(Guid studentId, Guid tutorId);
    Task<IEnumerable<StudentTutorRelation>> GetByTutorAsync(Guid tutorId, string? search = null);
    Task<IEnumerable<StudentTutorRelation>> GetByStudentAsync(Guid studentId, string? search = null);
    Task<int> GetByTutorCountAsync(Guid tutorId, string? search = null);
    Task<int> GetByStudentCountAsync(Guid studentId, string? search = null);
    Task<bool> DeleteByStudentAndTutorAsync(Guid studentId, Guid tutorId);
}