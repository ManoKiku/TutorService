using System.ComponentModel.DataAnnotations;
using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Phone]
    public string Phone { get; set; }

    [Required]
    public UserRole Role { get; set; }
}