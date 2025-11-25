using TutorService.Application.DTOs.Tag;
using TagDto = TutorService.Application.DTOs.Tag.TagDto;

namespace TutorService.Application.Intefaces;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetTagsAsync(string? search, int page, int pageSize);
    Task<TagDto?> GetTagByIdAsync(int id);
    Task<TagDto> CreateTagAsync(TagCreateDto createDto);
    Task<TagDto?> UpdateTagAsync(int id, TagUpdateDto updateDto);
    Task DeleteTagAsync(int id);
}