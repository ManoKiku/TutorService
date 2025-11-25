using System.ComponentModel.DataAnnotations;

namespace TutorService.Application.DTOs.User;

public class UserUpdateRequest
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Phone]
    public string Phone { get; set; }
}