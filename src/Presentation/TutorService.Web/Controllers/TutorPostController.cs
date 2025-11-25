using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutorService.Application.DTOs.Tutor;
using TutorService.Application.Intefaces;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/tutor-posts")]
public class TutorPostController : ControllerBase
{
    private readonly ITutorPostService _postService;
    private readonly ITutorProfileRepository _profileService;

    public TutorPostController(ITutorPostService postService, ITutorProfileRepository  profileService)
    {
        _postService = postService;
        _profileService = profileService;
    }

    [HttpPost]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> Create([FromBody] TutorPostCreateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);

        var tutorProfile = await _profileService.GetByUserIdAsync(userId);

        if (tutorProfile is null)
        {
            return Unauthorized("No tutor profile found.");
        }
        
        var dto = await _postService.CreateAsync(tutorProfile.Id, request);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _postService.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] TutorPostUpdateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var updated = await _postService.UpdateAsync(userId, id, request);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        await _postService.DeleteAsync(userId, id);
        return NoContent();
    }

    [HttpPost("{id}/tags")]
    [Authorize]
    public async Task<IActionResult> AddTags(Guid id, [FromBody] IEnumerable<int> tagIds)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        await _postService.AddTagsAsync(userId, id, tagIds);
        return NoContent();
    }

    [HttpDelete("{id}/tags/{tagId}")]
    [Authorize]
    public async Task<IActionResult> RemoveTag(Guid id, int tagId)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        await _postService.RemoveTagAsync(userId, id, tagId);
        return NoContent();
    }

    [HttpGet("{id}/tags")]
    public async Task<IActionResult> GetTags(Guid id)
    {
        var tags = await _postService.GetTagsAsync(id);
        return Ok(tags);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] int? subjectId, [FromQuery] int? cityId, [FromQuery] string? tags, [FromQuery] PostStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        IEnumerable<int>? tagIds = null;
        if (!string.IsNullOrWhiteSpace(tags)) tagIds = tags.Split(',').Select(int.Parse);

        var (results, total) = await _postService.SearchAsync(subjectId, cityId, tagIds, status, page, pageSize, search);
        return Ok(new { results, total });
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> MyPosts([FromQuery] PostStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var (results, total) = await _postService.GetMyPostsAsync(userId, status, page, pageSize);
        return Ok(new { results, total });
    }

    [HttpPost("{id}/moderate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Moderate(Guid id, [FromQuery] PostStatus status)
    {
        var adminId = ControllerHelper.GetUserIdFromClaims(User);
        await _postService.ModerateAsync(id, status, adminId);
        return NoContent();
    }
}
