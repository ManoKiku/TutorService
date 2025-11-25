using TutorService.Application.DTOs.Category;

namespace TutorService.Application.Intefaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryCreateRequest request);
    Task<CategoryDto> UpdateAsync(int id, CategoryCreateRequest request);
    Task DeleteAsync(int id);
}
