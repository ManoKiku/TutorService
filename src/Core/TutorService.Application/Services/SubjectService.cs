using AutoMapper;
using TutorService.Application.DTOs.Subject;
using TutorService.Application.Intefaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;
    private readonly IMapper _mapper;

    public SubjectService(ISubjectRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<SubjectDto> Results, int TotalCount)> SearchAsync(string? search, int page, int pageSize)
    {
        var (results, total) = await _repo.SearchAsync(search, page, pageSize);
        return (results.Select(s => _mapper.Map<SubjectDto>(s)), total);
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;
        return _mapper.Map<SubjectDto>(s);
    }

    public async Task<SubjectDto> CreateAsync(SubjectCreateRequest request)
    {
        var entity = new Subject { Name = request.Name, SubcategoryId = request.SubcategoryId };
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<SubjectDto>(created);
    }

    public async Task<SubjectDto> UpdateAsync(int id, SubjectCreateRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Subject not found");
        existing.Name = request.Name;
        existing.SubcategoryId = request.SubcategoryId;
        var updated = await _repo.UpdateAsync(existing);
        return _mapper.Map<SubjectDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Subject not found");
        await _repo.DeleteAsync(existing);
    }
}
