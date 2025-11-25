using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.City;
using TutorService.Application.DTOs.Tutor;
using TutorService.Application.Intefaces;
using TutorService.Domain.Enums;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class TutorProfileService : ITutorProfileService
{
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TutorProfileService> _logger;

    public TutorProfileService(IUserRepository userRepository, ITutorProfileRepository tutorProfileRepository, IMapper mapper, ILogger<TutorProfileService> logger)
    {
        _tutorProfileRepository = tutorProfileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TutorProfileDto?> GetByUserIdAsync(Guid userId)
    {
        var profile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        if (profile == null) return null;

        return _mapper.Map<TutorProfileDto>(profile);
    }

    public async Task<TutorProfileDto> UpsertAsync(Guid userId, TutorProfileUpdateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user.Role != UserRole.Tutor)
        {
            throw new UnauthorizedAccessException("User is not a tutor!");
        }
        
        var profile = new TutorProfile
        {
            UserId = userId,
            Bio = request.Bio,
            Education = request.Education,
            ExperienceYears = request.ExperienceYears,
            HourlyRate = request.HourlyRate
        };

        var updated = await _tutorProfileRepository.UpsertAsync(profile);
        return _mapper.Map<TutorProfileDto>(updated);
    }

    public async Task<TutorProfileDto?> GetByIdAsync(Guid id)
    {
        var profile = await _tutorProfileRepository.GetWithDetailsAsync(id);
        if (profile == null) return null;
        return _mapper.Map<TutorProfileDto>(profile);
    }

    public async Task<(IEnumerable<TutorProfileDto> Results, int TotalCount)> SearchAsync(
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
        var (results, total) = await _tutorProfileRepository.SearchAsync(categoryId, subcategoryId, subjectId, cityId, tagIds, minRate, maxRate, page, pageSize, search);
        return (results.Select(r => _mapper.Map<TutorProfileDto>(r)), total);
    }

    public async Task AddCityAsync(Guid userId, int cityId)
    {
        var profile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        if (profile == null) throw new KeyNotFoundException("Tutor profile not found");
        await _tutorProfileRepository.AddCityAsync(profile.Id, cityId);
    }

    public async Task RemoveCityAsync(Guid userId, int cityId)
    {
        var profile = await _tutorProfileRepository.GetByUserIdAsync(userId);
        if (profile == null) throw new KeyNotFoundException("Tutor profile not found");
        await _tutorProfileRepository.RemoveCityAsync(profile.Id, cityId);
    }

    public async Task<IEnumerable<CityDto>> GetCitiesAsync(Guid tutorProfileId)
    {
        var cities = await _tutorProfileRepository.GetCitiesAsync(tutorProfileId);
        return cities.Select(tc => new CityDto { Id = tc.City.Id, Name = tc.City.Name });
    }
}
