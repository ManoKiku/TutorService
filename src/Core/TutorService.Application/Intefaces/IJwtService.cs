using System.Security.Claims;
using TutorService.Domain.Entities;

namespace TutorService.Application.Intefaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    DateTime GetRefreshTokenExpiryDate();
}