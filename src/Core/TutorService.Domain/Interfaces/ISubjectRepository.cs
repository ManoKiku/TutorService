using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface ISubjectRepository : ICrudRepository<Subject>
{
    Task<(IEnumerable<Subject> Results, int TotalCount)> SearchAsync(string? search, int page, int pageSize);
}
