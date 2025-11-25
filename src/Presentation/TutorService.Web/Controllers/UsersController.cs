using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorService.Application.DTOs;
using TutorService.Application.DTOs.Auth;
using TutorService.Application.DTOs.User;
using TutorService.Application.DTOs.Tutor;
using TutorService.Application.Intefaces;
using TutorService.Application.Services;
using TutorService.Web.Attributes;
using TutorService.Web.Helpers;

namespace TutorService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("profile")]
    [ValidateModel]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var id = ControllerHelper.GetUserIdFromClaims(User);

        var user = await _userService.UpdateUserAsync(id, request);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (result)
        {
            return NoContent();
        }
        return BadRequest(new { message = "Failed to delete user" });
    }

    [HttpGet("{id}/tutor-profile")]
    [Authorize]
    public async Task<ActionResult<TutorProfileDto>> GetUserTutorProfile(Guid id, [FromServices] ITutorProfileService tutorProfileService)
    {
        var profile = await tutorProfileService.GetByUserIdAsync(id);
        if (profile == null)
        {
            return NotFound(new { message = "Tutor profile not found" });
        }
        return Ok(profile);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var id = ControllerHelper.GetUserIdFromClaims(User);
        
        var result = await _authService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);
        if (result)
        {
            return Ok(new { message = "Password changed successfully" });
        }
        return BadRequest(new { message = "Failed to change password" });
    }
}