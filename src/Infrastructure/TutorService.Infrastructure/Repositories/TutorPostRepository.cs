using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Domain.Enums;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class TutorPostRepository : BaseRepository<TutorPost>, ITutorPostRepository
{
    public TutorPostRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TutorPost> CreateAsync(TutorPost post)
    {
        await _dbSet.AddAsync(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<TutorPost> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Tutor)
                .ThenInclude(t => t.TutorCities)
            .Include(p => p.TutorPostTags)
                .ThenInclude(tpt => tpt.Tag)
            .Include(p => p.Subject)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<TutorPost> UpdateAsync(TutorPost post)
    {
        _dbSet.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task DeleteAsync(TutorPost post)
    {
        post.IsDeleted = true;
        _dbSet.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task AddTagsAsync(Guid postId, IEnumerable<int> tagIds)
    {
        var post = await _dbSet.Include(p => p.TutorPostTags).FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        if (post == null) throw new KeyNotFoundException("Post not found");

        foreach (var tagId in tagIds)
        {
            if (!post.TutorPostTags.Any(t => t.TagId == tagId))
            {
                post.TutorPostTags.Add(new TutorPostTag { TagId = tagId, TutorPostId = post.Id });
            }
        }

        _dbSet.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveTagAsync(Guid postId, int tagId)
    {
        var tpt = await _context.Set<TutorPostTag>().FirstOrDefaultAsync(t => t.TutorPostId == postId && t.TagId == tagId);
        if (tpt == null) throw new KeyNotFoundException("Tag relation not found");
        _context.Set<TutorPostTag>().Remove(tpt);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Tag>> GetTagsAsync(Guid postId)
    {
        return await _context.Set<TutorPostTag>()
            .Where(t => t.TutorPostId == postId)
            .Include(t => t.Tag)
            .Select(t => t.Tag)
            .ToListAsync();
    }

    public async Task<(IEnumerable<TutorPost> Results, int TotalCount)> SearchAsync(
        int? subjectId,
        int? cityId,
        IEnumerable<int>? tagIds,
        PostStatus? status,
        int page,
        int pageSize,
        Guid? tutorId,
        string? search)
    {
        var query = _dbSet
            .AsQueryable()
            .Where(p => !p.IsDeleted)
            .Include(p => p.Tutor)
                .ThenInclude(t => t.TutorCities)
            .Include(p => p.TutorPostTags)
                .ThenInclude(tpt => tpt.Tag)
            .Include(p => p.Subject)
            .AsQueryable();

        if (subjectId.HasValue)
            query = query.Where(p => p.SubjectId == subjectId.Value);

        if (tutorId.HasValue)
            query = query.Where(p => p.TutorId == tutorId.Value);

        if (cityId.HasValue)
            query = query.Where(p => p.Tutor.TutorCities.Any(tc => tc.CityId == cityId.Value));

        if (tagIds != null && tagIds.Any())
            query = query.Where(p => p.TutorPostTags.Any(tpt => tagIds.Contains(tpt.TagId)));

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);
        else
            query = query.Where(p => p.Status == PostStatus.Approved);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p => p.Description.ToLower().Contains(s) || p.Tutor.Bio.ToLower().Contains(s));
        }

        var total = await query.CountAsync();

        var results = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (results, total);
    }

    public async Task<(IEnumerable<TutorPost> Results, int TotalCount)> GetMyPostsAsync(Guid tutorId, PostStatus? status, int page, int pageSize)
    {
        var query = _dbSet.Where(p => !p.IsDeleted && p.TutorId == tutorId);
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);

        var total = await query.CountAsync();
        var results = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (results, total);
    }

    public async Task ModerateAsync(Guid id, PostStatus status, Guid adminId)
    {
        var post = await _dbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (post == null) throw new KeyNotFoundException("Post not found");
        post.Status = status;
        _dbSet.Update(post);
        await _context.SaveChangesAsync();
    }
}
