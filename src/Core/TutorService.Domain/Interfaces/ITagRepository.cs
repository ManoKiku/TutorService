using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

public interface ITagRepository : ICrudRepository<Tag>
{
    Task<IEnumerable<int>> GetExistingTagIdsAsync(IEnumerable<int> ids);
}