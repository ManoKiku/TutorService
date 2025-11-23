namespace TutorService.Infrastructure.Repositories;

using TutorService.Domain.Entities;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.TutorProfiles)
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Where(u => u.Role == role && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<IEnumerable<User>> GetTutorsWithProfilesAsync()
    {
        return await _dbSet
            .Where(u => u.Role == UserRole.Tutor && !u.IsDeleted)
            .Include(u => u.TutorProfiles)
            .ThenInclude(t => t.TutorCities)
            .ThenInclude(tc => tc.City)
            .Include(u => u.TutorProfiles)
            .ThenInclude(t => t.TutorPosts)
            .ThenInclude(p => p.Subject)
            .ToListAsync();
    }
}
