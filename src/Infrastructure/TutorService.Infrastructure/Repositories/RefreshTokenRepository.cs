using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetByIdAsync(Guid id)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Id == id && !rt.IsDeleted);
    }

    public async Task<IEnumerable<RefreshToken>> GetAllAsync()
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => !rt.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> FindAsync(System.Linq.Expressions.Expression<Func<RefreshToken, bool>> predicate)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .Where(predicate)
            .Where(rt => !rt.IsDeleted)
            .ToListAsync();
    }

    public async Task AddAsync(RefreshToken entity)
    {
        await _context.RefreshTokens.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<RefreshToken> entities)
    {
        await _context.RefreshTokens.AddRangeAsync(entities);
    }

    public void Update(RefreshToken entity)
    {
        _context.RefreshTokens.Update(entity);
    }

    public void Remove(RefreshToken entity)
    {
        _context.RefreshTokens.Remove(entity);
    }

    public void RemoveRange(IEnumerable<RefreshToken> entities)
    {
        _context.RefreshTokens.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsDeleted);
    }

    public async Task<RefreshToken> GetByJwtIdAsync(string jwtId)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId && !rt.IsDeleted);
    }

    public async Task InvalidateUserTokensAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsDeleted && !rt.Invalidated)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Invalidated = true;
        }

        _context.RefreshTokens.UpdateRange(tokens);
    }

    public async Task<bool> IsTokenValidAsync(string token)
    {
        var storedToken = await GetByTokenAsync(token);
        return storedToken != null && 
               !storedToken.Used && 
               !storedToken.Invalidated && 
               storedToken.ExpiryDate > DateTime.UtcNow;
    }
}