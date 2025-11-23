using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; }
}