namespace TutorService.Domain.Interfaces;

using TutorService.Domain.Entities;
using TutorService.Domain.Enums;

public interface ITutorProfileRepository : IRepository<TutorProfile>
{
    Task<TutorProfile> GetByUserIdAsync(Guid userId);
    Task<TutorProfile> GetWithDetailsAsync(Guid id);
    
    // Create or update tutor profile
    Task<TutorProfile> UpsertAsync(TutorProfile profile);

    // Manage tutor cities
    Task AddCityAsync(Guid tutorProfileId, int cityId);
    Task RemoveCityAsync(Guid tutorProfileId, int cityId);
    Task<IEnumerable<TutorCity>> GetCitiesAsync(Guid tutorProfileId);

    // Search tutors with filters and paging. Returns results and total count.
    Task<(IEnumerable<TutorProfile> Results, int TotalCount)> SearchAsync(
        int? categoryId,
        int? subcategoryId,
        int? subjectId,
        int? cityId,
        IEnumerable<int>? tagIds,
        decimal? minRate,
        decimal? maxRate,
        int page,
        int pageSize,
        string? search);
}