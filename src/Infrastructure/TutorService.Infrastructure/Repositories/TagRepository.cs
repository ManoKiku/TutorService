using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;


public class TagRepository : CrudRepository<Tag>, ITagRepository
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<int>> GetExistingTagIdsAsync(IEnumerable<int> ids)
    {
        return await _dbSet
            .Where(t => ids.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();
    }
}