using AutoMapper;
using TutorService.Application.DTOs.Category;
using TutorService.Application.Intefaces;
using TutorService.Domain.Entities;
using TutorService.Domain.Interfaces;

namespace TutorService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICrudRepository<Category> _repo;
    private readonly IMapper _mapper;

    public CategoryService(ICrudRepository<Category> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var list = await _repo.GetAllAsync();
        return list.Select(c => _mapper.Map<CategoryDto>(c));
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;
        return _mapper.Map<CategoryDto>(c);
    }

    public async Task<CategoryDto> CreateAsync(CategoryCreateRequest request)
    {
        var entity = new Category { Name = request.Name };
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<CategoryDto>(created);
    }

    public async Task<CategoryDto> UpdateAsync(int id, CategoryCreateRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Category not found");
        existing.Name = request.Name;
        var updated = await _repo.UpdateAsync(existing);
        return _mapper.Map<CategoryDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException("Category not found");
        await _repo.DeleteAsync(existing);
    }
}
