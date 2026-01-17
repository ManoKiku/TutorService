using AutoMapper;
using Microsoft.Extensions.Logging;
using TutorService.Application.DTOs.Assigment;
using TutorService.Application.Interfaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IFileRepository _fileRepository;
    private readonly ITutorProfileRepository _tutorProfileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AssignmentService> _logger;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        ILessonRepository lessonRepository,
        IFileRepository fileRepository,
        IMapper mapper,
        ILogger<AssignmentService> logger,
        ITutorProfileRepository tutorProfileRepository)
    {
        _assignmentRepository = assignmentRepository;
        _lessonRepository = lessonRepository;
        _fileRepository = fileRepository;
        _mapper = mapper;
        _logger = logger;
        _tutorProfileRepository = tutorProfileRepository;
    }

    public async Task<AssignmentDto> CreateAsync(Guid tutorId, AssignmentCreateRequest request)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(tutorId);
        
        if(tutorProfile is null)
            throw new KeyNotFoundException("Tutor not found");
        
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        if (lesson.TutorId != tutorProfile.Id)
            throw new UnauthorizedAccessException("You can only create assignments for your own lessons");

        if (request.File == null || request.File.Length == 0)
            throw new ArgumentException("File is required");

        if (request.File.Length > 50 * 1024 * 1024)
            throw new ArgumentException("File size cannot exceed 50MB");

        var allowedExtensions = new[] { 
            ".pdf", ".doc", ".docx", ".txt", ".zip", ".rar", 
            ".jpg", ".jpeg", ".png", ".pptx", ".xlsx", ".ppt", ".xls" 
        };
        
        var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Invalid file type");

        string mongoFileId;
        using (var fileStream = request.File.OpenReadStream())
        {
            mongoFileId = await _fileRepository.UploadFileAsync(request.File.FileName, fileStream);
        }

        var assignment = new Assignment
        {
            LessonId = request.LessonId,
            FileName = request.File.FileName,
            MongoFileId = mongoFileId,
            FileSize = request.File.Length,
            ContentType = request.File.ContentType,
            UploadedAt = DateTime.UtcNow
        };

        await _assignmentRepository.AddAsync(assignment);
        await _assignmentRepository.SaveChangesAsync();
        
        var assignmentWithDetails = await _assignmentRepository.GetByIdWithDetailsAsync(assignment.Id);
        
        return _mapper.Map<AssignmentDto>(assignmentWithDetails!);
    }

    public async Task<IEnumerable<AssignmentDto>> GetByLessonIdAsync(Guid lessonId, Guid currentUserId, string currentUserRole)
    {
        if (currentUserRole != "Admin")
        {
            var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
        
            if(tutorProfile is null && currentUserRole == "Tutor")
                throw new KeyNotFoundException("Tutor not found");

            var isParticipant = await _lessonRepository.IsUserParticipantAsync(lessonId,
                tutorProfile?.Id ?? currentUserId);
            if (!isParticipant)
                throw new ArgumentException("You don't have access to this lesson's assignments");
        }

        var assignments = await _assignmentRepository.GetByLessonIdAsync(lessonId);
        return _mapper.Map<IEnumerable<AssignmentDto>>(assignments);
    }

    public async Task<AssignmentDto?> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);
        if (assignment == null)
            return null;
        
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
        
        if(tutorProfile is null && currentUserRole == "Tutor")
            throw new KeyNotFoundException("Tutor not found");
        
        if (currentUserRole != "Admin" && !await _assignmentRepository.IsUserParticipantAsync(id, tutorProfile == null ? currentUserId : tutorProfile.Id))
            throw new UnauthorizedAccessException("You don't have access to this assignment");

        return _mapper.Map<AssignmentDto>(assignment);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, string currentUserRole)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);
        if (assignment == null)
            return false;
        
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
        
        if(tutorProfile is null && currentUserRole != "Admin")
            throw new KeyNotFoundException("Tutor not found");

        if (currentUserRole != "Admin" && assignment.Lesson!.TutorId != tutorProfile.Id)
            throw new UnauthorizedAccessException("You can only delete your own assignments");

        if (!string.IsNullOrEmpty(assignment.MongoFileId))
        {
            await _fileRepository.DeleteFileAsync(assignment.MongoFileId);
        }

        _assignmentRepository.Remove(assignment);
        await _assignmentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<FileDownloadResponse> DownloadFileAsync(Guid id, Guid currentUserId, string currentUserRole)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);
        if (assignment == null)
            throw new KeyNotFoundException("Assignment not found");
        
        var tutorProfile = await _tutorProfileRepository.GetByUserIdAsync(currentUserId);
        
        if(tutorProfile is null && currentUserRole == "Tutor")
            throw new KeyNotFoundException("Tutor not found");

        if (currentUserRole != "Admin" && !await _assignmentRepository.IsUserParticipantAsync(id, tutorProfile?.Id ?? currentUserId))
            throw new UnauthorizedAccessException("You don't have access to this assignment");


        if (string.IsNullOrEmpty(assignment.MongoFileId))
            throw new InvalidOperationException("Assignment doesn't have a file");

        var fileStream = await _fileRepository.DownloadFileAsync(assignment.MongoFileId);
        var fileInfo = await _fileRepository.GetFileInfoAsync(assignment.MongoFileId);

        return new FileDownloadResponse
        {
            FileStream = fileStream,
            ContentType = assignment.ContentType,
            FileName = assignment.FileName,
            FileSize = assignment.FileSize
        };
    }
}