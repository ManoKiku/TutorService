using Microsoft.EntityFrameworkCore;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class StudentTutorRelationRepository : CrudRepository<StudentTutorRelation>, IStudentTutorRelationRepository
{
    public StudentTutorRelationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> RelationExistsAsync(Guid studentId, Guid tutorId)
    {
        return await _dbSet
            .AnyAsync(r => r.StudentId == studentId && r.TutorId == tutorId);
    }

    public async Task<StudentTutorRelation?> GetByStudentAndTutorAsync(Guid studentId, Guid tutorId)
    {
        return await _dbSet
            .Include(r => r.Student)
            .Include(r => r.Tutor)
            .ThenInclude(t => t!.User)
            .FirstOrDefaultAsync(r => r.StudentId == studentId && r.TutorId == tutorId);
    }

    public async Task<IEnumerable<StudentTutorRelation>> GetByTutorAsync(Guid tutorId, string? search = null)
    {
        var query = _dbSet
            .Include(r => r.Student)
            .Where(r => r.TutorId == tutorId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => 
                r.Student!.FirstName.Contains(search) || 
                r.Student!.LastName.Contains(search) ||
                r.Student!.Email.Contains(search));
        }

        return await query
            .OrderByDescending(r => r.AddedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentTutorRelation>> GetByStudentAsync(Guid studentId, string? search = null)
    {
        var query = _dbSet
            .Include(r => r.Tutor)
            .ThenInclude(t => t!.User)
            .Where(r => r.StudentId == studentId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => 
                r.Tutor!.User!.FirstName.Contains(search) || 
                r.Tutor!.User!.LastName.Contains(search) ||
                r.Tutor!.User!.Email.Contains(search));
        }

        return await query
            .OrderByDescending(r => r.AddedAt)
            .ToListAsync();
    }

    public async Task<int> GetByTutorCountAsync(Guid tutorId, string? search = null)
    {
        var query = _dbSet.Where(r => r.TutorId == tutorId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => 
                r.Student!.FirstName.Contains(search) || 
                r.Student!.LastName.Contains(search) ||
                r.Student!.Email.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task<int> GetByStudentCountAsync(Guid studentId, string? search = null)
    {
        var query = _dbSet.Where(r => r.StudentId == studentId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => 
                r.Tutor!.User!.FirstName.Contains(search) || 
                r.Tutor!.User!.LastName.Contains(search) ||
                r.Tutor!.User!.Email.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task<bool> DeleteByStudentAndTutorAsync(Guid studentId, Guid tutorId)
    {
        var relation = await GetByStudentAndTutorAsync(studentId, tutorId);
        if (relation == null)
            return false;

        await DeleteAsync(relation);
        return true;
    }
}
