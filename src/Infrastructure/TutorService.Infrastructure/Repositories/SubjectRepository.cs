using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class SubjectRepository : CrudRepository<Subject>, ISubjectRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Subject> _dbSet;

    public SubjectRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
        _dbSet = context.Set<Subject>();
    }

    public async Task<(IEnumerable<Subject> Results, int TotalCount)> SearchAsync(string? search, int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(s));
        }

        var total = await query.CountAsync();
        var results = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (results, total);
    }
}
