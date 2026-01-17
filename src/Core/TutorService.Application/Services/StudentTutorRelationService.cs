using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.StudentTutorRelation;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class StudentTutorRelationService : IStudentTutorRelationService
{
    private readonly IStudentTutorRelationRepository _relationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentTutorRelationService> _logger;

    public StudentTutorRelationService(
        IStudentTutorRelationRepository relationRepository,
        IUserRepository userRepository,
        ITutorProfileRepository tutorProfileRepository,
        IMapper mapper,
        ILogger<StudentTutorRelationService> logger)
    {
        _relationRepository = relationRepository;
        _userRepository = userRepository;
        _tutorProfileRepository = tutorProfileRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<StudentTutorRelationDto> CreateRelationAsync(Guid tutorUserId, StudentTutorRelationCreateRequest request)
    {
        var tutor = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        if (tutor == null)
            throw new KeyNotFoundException("Tutor profile not found");

        var student = await _userRepository.GetByIdAsync(request.StudentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found");

        if (await _relationRepository.RelationExistsAsync(request.StudentId, tutor.Id))
            throw new InvalidOperationException("Relation already exists");

        var relation = new StudentTutorRelation
        {
            StudentId = request.StudentId,
            TutorId = tutor.Id,
            AddedAt = DateTime.UtcNow
        };

        var createdRelation = await _relationRepository.CreateAsync(relation);
        var relationWithDetails = await _relationRepository.GetByStudentAndTutorAsync(request.StudentId, tutor.Id);
        
        return _mapper.Map<StudentTutorRelationDto>(relationWithDetails!);
    }

    public async Task<IEnumerable<StudentTutorRelationDto>> GetMyStudentsAsync(Guid tutorUserId, string? search = null)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found");
        
        var relations = await _relationRepository.GetByTutorAsync(tutorProfile.Id, search);
        var totalCount = await _relationRepository.GetByTutorCountAsync(tutorProfile.Id, search);

        return _mapper.Map<IEnumerable<StudentTutorRelationDto>>(relations);
    }

    public async Task<IEnumerable<StudentTutorRelationDto>> GetMyTutorsAsync(Guid studentId, string? search = null)
    {
        var relations = await _relationRepository.GetByStudentAsync(studentId, search);
        var totalCount = await _relationRepository.GetByStudentCountAsync(studentId, search);

        return _mapper.Map<IEnumerable<StudentTutorRelationDto>>(relations);
    }

    public async Task<bool> DeleteRelationAsync(Guid tutorUserId, Guid studentId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorUserId);
        
        if (tutorProfile == null)
            throw new KeyNotFoundException("Tutor profile not found");
        
        return await _relationRepository.DeleteByStudentAndTutorAsync(studentId, tutorProfile.Id);
    }

    public async Task<StudentTutorRelationDto> CheckRelationAsync(Guid studentId, Guid tutorId)
    {
        var tutorProfile = await _tutorProfileRepository.GetByIdAsync(tutorId);
        
        if (tutorProfile == null)
            throw new ArgumentException("Tutor profile not found");

        var relation = await _relationRepository.GetByStudentAndTutorAsync(studentId, tutorProfile.Id);
        
        if(relation == null)
            throw new KeyNotFoundException("Relation not found");

        return _mapper.Map<StudentTutorRelationDto>(relation);
    }

    public async Task<bool> AreRelatedAsync(Guid studentId, Guid tutorId)
    {
        return await _relationRepository.RelationExistsAsync(studentId, tutorId);
    }
}