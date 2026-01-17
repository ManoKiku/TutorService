using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.Lesson;
using TutorService.Application.Interfaces;
using TutorService.Domain.Enums;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService, ILogger<LessonsController> logger)
    {
        _lessonService = lessonService;
    }
    

    [HttpPost]
    [Authorize(Roles = "Tutor")]
    public async Task<ActionResult<LessonDto>> CreateLesson(LessonCreateRequest request)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var lessonDto = await _lessonService.CreateAsync(tutorId, request);
        return CreatedAtAction(nameof(GetLesson), new { id = lessonDto.Id }, lessonDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessons(
        [FromQuery] Guid? userId,
        [FromQuery] LessonStatus? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? tutorId,
        [FromQuery] Guid? studentId)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var response = await _lessonService.GetLessonsAsync(
            currentUserId, currentUserRole, userId, status, startDate, endDate, 
            tutorId, studentId);
            
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var lesson = await _lessonService.GetByIdAsync(id, currentUserId, currentUserRole);
        if (lesson == null)
            return NotFound();

        return Ok(lesson);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(Guid id, LessonUpdateRequest request)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var lesson = await _lessonService.UpdateAsync(id, request, currentUserId, currentUserRole);
        return Ok(lesson);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson(Guid id)
    {
        var currentUserId = ControllerHelper.GetUserIdFromClaims(User);
        var currentUserRole = ControllerHelper.GetUserRoleFromClaims(User);
        
        var result = await _lessonService.DeleteAsync(id, currentUserId, currentUserRole);
        
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("upcoming")]
    [Authorize(Roles = "Student,Tutor")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetUpcomingLessons(
        [FromQuery] int daysAhead = 7)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var lessons = await _lessonService.GetUpcomingLessonsAsync(userId, daysAhead);
        return Ok(lessons);
    }

    [HttpGet("calendar")]
    [Authorize(Roles = "Student,Tutor")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetCalendarLessons(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(User);
        var lessons = await _lessonService.GetCalendarLessonsAsync(userId, month, year);
        return Ok(lessons);
    }
}