using System.ComponentModel.DataAnnotations;

namespace TutorService.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}