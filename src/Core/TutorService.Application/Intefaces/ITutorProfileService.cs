using TutorService.Application.DTOs.City;
using TutorService.Application.DTOs.Tutor;

namespace TutorService.Application.Intefaces;

public interface ITutorProfileService
{
    Task<TutorProfileDto?> GetByUserIdAsync(Guid userId);
    Task<TutorProfileDto> UpsertAsync(Guid userId, TutorProfileUpdateRequest request);
    Task<TutorProfileDto?> GetByIdAsync(Guid id);
    Task<(IEnumerable<TutorProfileDto> Results, int TotalCount)> SearchAsync(
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

    Task AddCityAsync(Guid userId, int cityId);
    Task RemoveCityAsync(Guid userId, int cityId);
    Task<IEnumerable<CityDto>> GetCitiesAsync(Guid tutorProfileId);
}
