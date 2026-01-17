using Microsoft.EntityFrameworkCore;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;

namespace TutorService.Infrastructure.Repositories;

public class TutorProfileRepository : BaseRepository<TutorProfile>, ITutorProfileRepository
{
    public TutorProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TutorProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(tp => tp.TutorCities)
                .ThenInclude(tc => tc.City)
            .Include(tp => tp.TutorPosts)
                .ThenInclude(p => p.Subject)
            .Include(tp => tp.Lessons)
            .Include(tp => tp.Chats)
            .FirstOrDefaultAsync(tp => tp.UserId == userId && !tp.IsDeleted);
    }

    public async Task<TutorProfile?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Where(tp => tp.Id == id && !tp.IsDeleted)
            .Include(tp => tp.TutorCities)
                .ThenInclude(tc => tc.City)
            .Include(tp => tp.TutorPosts)
                .ThenInclude(p => p.Subject)
            .Include(tp => tp.Lessons)
            .Include(tp => tp.Chats)
            .Include(tp => tp.User)
            .FirstOrDefaultAsync();
    }

    public async Task<TutorProfile> UpsertAsync(TutorProfile profile)
    {
        if (profile == null) throw new ArgumentNullException(nameof(profile));

        var existing = await _dbSet.FirstOrDefaultAsync(tp => tp.UserId == profile.UserId && !tp.IsDeleted);
        if (existing == null)
        {
            await _dbSet.AddAsync(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        existing.Bio = profile.Bio;
        existing.Education = profile.Education;
        existing.ExperienceYears = profile.ExperienceYears;
        existing.HourlyRate = profile.HourlyRate;

        _dbSet.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task AddCityAsync(Guid tutorProfileId, int cityId)
    {
        var profile = await _dbSet
            .Include(tp => tp.TutorCities)
            .FirstOrDefaultAsync(tp => tp.Id == tutorProfileId && !tp.IsDeleted);

        if (profile == null) throw new KeyNotFoundException("Tutor profile not found");

        if (!profile.TutorCities.Any(tc => tc.CityId == cityId))
        {
            profile.TutorCities.Add(new TutorCity { CityId = cityId, TutorId = profile.Id });
            _dbSet.Update(profile);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveCityAsync(Guid tutorProfileId, int cityId)
    {
        var tutorCity = await _context.TutorCities
            .FirstOrDefaultAsync(tc => tc.TutorId == tutorProfileId && tc.CityId == cityId);

        if (tutorCity == null) throw new KeyNotFoundException("Tutor city relation not found");

        _context.TutorCities.Remove(tutorCity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TutorCity>> GetCitiesAsync(Guid tutorProfileId)
    {
        return await _context.TutorCities
            .Include(tc => tc.City)
            .Where(tc => tc.TutorId == tutorProfileId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<TutorProfile> Results, int TotalCount)> SearchAsync(
        int? categoryId,
        int? subcategoryId,
        int? subjectId,
        int? cityId,
        IEnumerable<int>? tagIds,
        decimal? minRate,
        decimal? maxRate,
        int page,
        int pageSize,
        string? search)
    {
        var query = _dbSet
            .AsQueryable()
            .Where(tp => !tp.IsDeleted)
            .Include(tp => tp.TutorPosts)
                .ThenInclude(p => p.Subject)
            .Include(tp => tp.TutorCities)
                .ThenInclude(tc => tc.City)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(tp => tp.TutorPosts.Any(p => p.Subject!.Subcategory!.CategoryId == categoryId.Value));
        }

        if (subcategoryId.HasValue)
        {
            query = query.Where(tp => tp.TutorPosts.Any(p => p.Subject!.SubcategoryId == subcategoryId.Value));
        }

        if (subjectId.HasValue)
        {
            query = query.Where(tp => tp.TutorPosts.Any(p => p.SubjectId == subjectId.Value));
        }

        if (cityId.HasValue)
        {
            query = query.Where(tp => tp.TutorCities.Any(tc => tc.CityId == cityId.Value));
        }

        if (tagIds != null && tagIds.Any())
        {
            query = query.Where(tp => tp.TutorPosts.Any(p => p.TutorPostTags.Any(tpt => tagIds.Contains(tpt.TagId))));
        }

        if (minRate.HasValue)
        {
            query = query.Where(tp => tp.HourlyRate >= minRate.Value);
        }

        if (maxRate.HasValue)
        {
            query = query.Where(tp => tp.HourlyRate <= maxRate.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(tp => tp.Bio.ToLower().Contains(s) || tp.Education.ToLower().Contains(s) || tp.TutorPosts.Any(p => p.Description.ToLower().Contains(s)));
        }

        var total = await query.CountAsync();

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (results, total);
    }
}
