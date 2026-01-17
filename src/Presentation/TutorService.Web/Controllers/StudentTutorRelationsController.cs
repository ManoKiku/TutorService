using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs.StudentTutorRelation;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentTutorRelationsController : ControllerBase
{
    private readonly IStudentTutorRelationService _relationService;

    public StudentTutorRelationsController(
        IStudentTutorRelationService relationService,
        ILogger<StudentTutorRelationsController> logger)
    {
        _relationService = relationService;
    }

    [HttpPost]
    [Authorize(Roles = "Tutor")]
    public async Task<ActionResult<StudentTutorRelationDto>> CreateRelation(StudentTutorRelationCreateRequest request)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var relation = await _relationService.CreateRelationAsync(tutorId, request);
        return CreatedAtAction(nameof(CheckRelation), new { studentId = request.StudentId, tutorId }, relation);
    }

    [HttpGet("my-students")]
    [Authorize(Roles = "Tutor")]
    public async Task<ActionResult<IEnumerable<StudentTutorRelationDto>>> GetMyStudents(
        [FromQuery] string? search)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var response = await _relationService.GetMyStudentsAsync(tutorId, search);
        return Ok(response);
    }

    [HttpGet("my-tutors")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IEnumerable<StudentTutorRelationDto>>> GetMyTutors(
        [FromQuery] string? search)
    {
        var studentId = ControllerHelper.GetUserIdFromClaims(User);
        var response = await _relationService.GetMyTutorsAsync(studentId, search);
        return Ok(response);
    }

    [HttpDelete("{studentId}")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> DeleteRelation(Guid studentId)
    {
        var tutorId = ControllerHelper.GetUserIdFromClaims(User);
        var result = await _relationService.DeleteRelationAsync(tutorId, studentId);
        
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("check")]
    [Authorize(Roles = "Student,Tutor")]
    public async Task<ActionResult<StudentTutorRelationDto>> CheckRelation(
        [FromQuery] Guid studentId,
        [FromQuery] Guid tutorId)
    {
        var response = await _relationService.CheckRelationAsync(studentId, tutorId);
        return Ok(response);
    }
}