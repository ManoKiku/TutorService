using TutorService.Application.DTOs.Subject;

namespace TutorService.Application.Intefaces;

public interface ISubjectService
{
    Task<(IEnumerable<SubjectDto> Results, int TotalCount)> SearchAsync(string? search, int page, int pageSize);
    Task<SubjectDto?> GetByIdAsync(int id);
    Task<SubjectDto> CreateAsync(SubjectCreateRequest request);
    Task<SubjectDto> UpdateAsync(int id, SubjectCreateRequest request);
    Task DeleteAsync(int id);
}
