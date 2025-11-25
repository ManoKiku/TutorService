using TutorService.Application.DTOs.Subcategory;

namespace TutorService.Application.Intefaces;

public interface ISubCategoryService 
{
    Task<IEnumerable<SubcategoryDto>> GetAllAsync();
    Task<SubcategoryDto?> GetByIdAsync(int id);
    Task<SubcategoryDto> CreateAsync(SubcategoryCreateRequest request);
    Task<SubcategoryDto> UpdateAsync(int id, SubcategoryCreateRequest request);
    Task DeleteAsync(int id);
}
