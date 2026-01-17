using TutorService.Application.DTOs.Tutor;
using TutorService.Domain.Enums;

namespace TutorService.Application.Interfaces;

public interface ITutorPostService
{
    Task<TutorPostDto> CreateAsync(Guid tutorProfileId, TutorPostCreateRequest request);
    Task<TutorPostDto?> GetByIdAsync(Guid id);
    Task<TutorPostDto> UpdateAsync(Guid tutorProfileId, Guid id, TutorPostUpdateRequest request);
    Task DeleteAsync(Guid tutorProfileId, Guid id);

    Task AddTagsAsync(Guid tutorProfileId, Guid postId, IEnumerable<int> tagIds);
    Task RemoveTagAsync(Guid tutorProfileId, Guid postId, int tagId);
    Task<IEnumerable<TagDto>> GetTagsAsync(Guid postId);

    Task<IEnumerable<TutorPostDto>> SearchAsync(
        int? subjectId,
        int? cityId,
        IEnumerable<int>? tagIds,
        PostStatus? status,
        string? search);

    Task<IEnumerable<TutorPostDto>> GetMyPostsAsync(Guid tutorProfileId, PostStatus? status);
    Task ModerateAsync(Guid id, PostStatus status, Guid adminId);
}
