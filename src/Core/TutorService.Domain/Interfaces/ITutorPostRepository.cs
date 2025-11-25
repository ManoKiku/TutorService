namespace TutorService.Domain.Interfaces;

using TutorService.Domain.Entities;
using TutorService.Domain.Enums;

public interface ITutorPostRepository : IRepository<TutorPost>
{
    Task<TutorPost> CreateAsync(TutorPost post);
    Task<TutorPost> GetByIdWithDetailsAsync(Guid id);
    Task<TutorPost> UpdateAsync(TutorPost post);
    Task DeleteAsync(TutorPost post);

    Task AddTagsAsync(Guid postId, IEnumerable<int> tagIds);
    Task RemoveTagAsync(Guid postId, int tagId);
    Task<IEnumerable<Tag>> GetTagsAsync(Guid postId);

    Task<(IEnumerable<TutorPost> Results, int TotalCount)> SearchAsync(
        int? subjectId,
        int? cityId,
        IEnumerable<int>? tagIds,
        PostStatus? status,
        int page,
        int pageSize,
        Guid? tutorId,
        string? search);

    Task<(IEnumerable<TutorPost> Results, int TotalCount)> GetMyPostsAsync(Guid tutorId, PostStatus? status, int page, int pageSize);

    Task ModerateAsync(Guid id, PostStatus status, Guid adminId);
}
