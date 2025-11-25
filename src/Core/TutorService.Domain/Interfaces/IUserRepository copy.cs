namespace TutorService.Domain.Interfaces;

using TutorService.Domain.Entities;
using TutorService.Domain.Enums;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    Task<bool> UserExistsAsync(string email);
    Task<IEnumerable<User>> GetTutorsWithProfilesAsync();
}
