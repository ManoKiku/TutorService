using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Tutor;
using TutorService.Application.Intefaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class TutorPostService : ITutorPostService
{
    private readonly ITutorPostRepository _postRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TutorPostService> _logger;

    public TutorPostService(
        ITutorPostRepository postRepository, 
        ITutorProfileRepository tutorProfileRepository, 
        ISubjectRepository subjectRepository,
        ITagRepository tagRepository,
        IMapper mapper, 
        ILogger<TutorPostService> logger)
    {
        _postRepository = postRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _mapper = mapper;
        _logger = logger;
        _subjectRepository = subjectRepository;
        _tagRepository = tagRepository;
    }

    public async Task<TutorPostDto> CreateAsync(Guid tutorProfileId, TutorPostCreateRequest request)
    {
        var tutor = await _tutorProfileRepository.GetWithDetailsAsync(tutorProfileId);
        if (tutor == null) throw new KeyNotFoundException("Tutor profile not found");
        
        var subjectExists = await _subjectRepository.GetByIdAsync(request.SubjectId) is not null;
        if (!subjectExists)
            throw new KeyNotFoundException($"Subject with ID {request.SubjectId} not found");
        
        if (request.TagIds != null && request.TagIds.Any())
        {
            var existingTags = await _tagRepository.GetExistingTagIdsAsync(request.TagIds);
            if (existingTags.Count() != request.TagIds.Count())
                throw new KeyNotFoundException("One or more tag IDs are invalid");
        }
        var post = new TutorPost
        {
            TutorId = tutorProfileId,
            SubjectId = request.SubjectId,
            Description = request.Description,
            Status = PostStatus.Pending
        };

        var created = await _postRepository.CreateAsync(post);
        if (request.TagIds != null && request.TagIds.Any())
            await _postRepository.AddTagsAsync(created.Id, request.TagIds);

        var withDetails = await _postRepository.GetByIdWithDetailsAsync(created.Id);
        return _mapper.Map<TutorPostDto>(withDetails);
    }

    public async Task<TutorPostDto?> GetByIdAsync(Guid id)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(id);
        if (post == null) return null;
        return _mapper.Map<TutorPostDto>(post);
    }

    public async Task<TutorPostDto> UpdateAsync(Guid tutorProfileId, Guid id, TutorPostUpdateRequest request)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(id);
        if (post == null) throw new KeyNotFoundException("Post not found");
        if (post.TutorId != tutorProfileId) throw new UnauthorizedAccessException("Not owner");

        post.SubjectId = request.SubjectId;
        post.Description = request.Description;

        var updated = await _postRepository.UpdateAsync(post);
        return _mapper.Map<TutorPostDto>(updated);
    }

    public async Task DeleteAsync(Guid tutorProfileId, Guid id)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(id);
        if (post == null) throw new KeyNotFoundException("Post not found");
        if (post.TutorId != tutorProfileId) throw new UnauthorizedAccessException("Not owner");
        await _postRepository.DeleteAsync(post);
    }

    public async Task AddTagsAsync(Guid tutorProfileId, Guid postId, IEnumerable<int> tagIds)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(postId);
        if (post == null) throw new KeyNotFoundException("Post not found");
        if (post.TutorId != tutorProfileId) throw new UnauthorizedAccessException("Not owner");
        await _postRepository.AddTagsAsync(postId, tagIds);
    }

    public async Task RemoveTagAsync(Guid tutorProfileId, Guid postId, int tagId)
    {
        var post = await _postRepository.GetByIdWithDetailsAsync(postId);
        if (post == null) throw new KeyNotFoundException("Post not found");
        if (post.TutorId != tutorProfileId) throw new UnauthorizedAccessException("Not owner");
        await _postRepository.RemoveTagAsync(postId, tagId);
    }

    public async Task<IEnumerable<TagDto>> GetTagsAsync(Guid postId)
    {
        var tags = await _postRepository.GetTagsAsync(postId);
        return tags.Select(t => _mapper.Map<TagDto>(t));
    }

    public async Task<(IEnumerable<TutorPostDto> Results, int TotalCount)> SearchAsync(int? subjectId, int? cityId, IEnumerable<int>? tagIds, PostStatus? status, int page, int pageSize, string? search)
    {
        var (results, total) = await _postRepository.SearchAsync(subjectId, cityId, tagIds, status, page, pageSize, null, search);
        return (results.Select(r => _mapper.Map<TutorPostDto>(r)), total);
    }

    public async Task<(IEnumerable<TutorPostDto> Results, int TotalCount)> GetMyPostsAsync(Guid tutorProfileId, PostStatus? status, int page, int pageSize)
    {
        var (results, total) = await _postRepository.GetMyPostsAsync(tutorProfileId, status, page, pageSize);
        return (results.Select(r => _mapper.Map<TutorPostDto>(r)), total);
    }

    public async Task ModerateAsync(Guid id, PostStatus status, Guid adminId)
    {
        await _postRepository.ModerateAsync(id, status, adminId);
        var post = await _postRepository.GetByIdWithDetailsAsync(id);
        if (post == null) throw new KeyNotFoundException("Post not found");
        await _postRepository.UpdateAsync(post);
    }
}
