using AutoMapper;
using TutorService.Application.DTOs.Subcategory;
using TutorService.Application.Intefaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class SubCategoryService : ISubCategoryService
{
    private readonly ICrudRepository<Subcategory> _repo;
    private readonly IMapper _mapper;

    public SubCategoryService(ICrudRepository<Subcategory> repo,  IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SubcategoryDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(s => _mapper.Map<SubcategoryDto>(s));
    }

    public async Task<SubcategoryDto?> GetByIdAsync(int id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;
        return _mapper.Map<SubcategoryDto>(s);
    }

    public async Task<SubcategoryDto> CreateAsync(SubcategoryCreateRequest request)
    {
        var entity = new Subcategory { Name = request.Name, CategoryId = request.CategoryId };
        var created = await _repo.CreateAsync(entity);
        return  _mapper.Map<SubcategoryDto>(created);
    }

    public async Task<SubcategoryDto> UpdateAsync(int id, SubcategoryCreateRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Subcategory not found");
        existing.Name = request.Name;
        existing.CategoryId = request.CategoryId;
        var updated = await _repo.UpdateAsync(existing);
        return _mapper.Map<SubcategoryDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Subcategory not found");
        await _repo.DeleteAsync(existing);
    }
}
