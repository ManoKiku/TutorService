using TutorService.Application.DTOs.Tutor;
using TutorService.Domain.Enums;

namespace TutorService.Application.Intefaces;

public interface ITutorPostService
{
    Task<TutorPostDto> CreateAsync(Guid tutorProfileId, TutorPostCreateRequest request);
    Task<TutorPostDto?> GetByIdAsync(Guid id);
    Task<TutorPostDto> UpdateAsync(Guid tutorProfileId, Guid id, TutorPostUpdateRequest request);
    Task DeleteAsync(Guid tutorProfileId, Guid id);

    Task AddTagsAsync(Guid tutorProfileId, Guid postId, IEnumerable<int> tagIds);
    Task RemoveTagAsync(Guid tutorProfileId, Guid postId, int tagId);
    Task<IEnumerable<TagDto>> GetTagsAsync(Guid postId);

    Task<(IEnumerable<TutorPostDto> Results, int TotalCount)> SearchAsync(
        int? subjectId,
        int? cityId,
        IEnumerable<int>? tagIds,
        PostStatus? status,
        int page,
        int pageSize,
        string? search);

    Task<(IEnumerable<TutorPostDto> Results, int TotalCount)> GetMyPostsAsync(Guid tutorProfileId, PostStatus? status, int page, int pageSize);
    Task ModerateAsync(Guid id, PostStatus status, Guid adminId);
}
