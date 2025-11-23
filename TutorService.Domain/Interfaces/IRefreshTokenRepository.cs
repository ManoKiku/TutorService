using TutorService.Domain.Entities;

namespace TutorService.Domain.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<RefreshToken> GetByJwtIdAsync(string jwtId);
    Task InvalidateUserTokensAsync(Guid userId);
    Task<bool> IsTokenValidAsync(string token);
}