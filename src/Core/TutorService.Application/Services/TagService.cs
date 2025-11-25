using AutoMapper;
using TutorService.Application.DTOs.Tag;
using TutorService.Application.Intefaces;
using TagDto = TutorService.Application.DTOs.Tag.TagDto;

namespace TutorService.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public TagService(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TagDto>> GetTagsAsync(string? search, int page, int pageSize)
    {
        var tags = await _tagRepository.GetAllAsync();
        
        if (!string.IsNullOrEmpty(search))
        {
            tags = tags.Where(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var pagedTags = tags
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return _mapper.Map<IEnumerable<TagDto>>(pagedTags);
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        return _mapper.Map<TagDto?>(tag);
    }

    public async Task<TagDto> CreateTagAsync(TagCreateDto createDto)
    {
        var tag = _mapper.Map<Domain.Entities.Tag>(createDto);
        var createdTag = await _tagRepository.CreateAsync(tag);
        return _mapper.Map<TagDto>(createdTag);
    }

    public async Task<TagDto> UpdateTagAsync(int id, TagUpdateDto updateDto)
    {
        var existingTag = await _tagRepository.GetByIdAsync(id);
        if (existingTag == null)
            throw new KeyNotFoundException($"Tag with ID {id} not found");

        _mapper.Map(updateDto, existingTag);
        var updatedTag = await _tagRepository.UpdateAsync(existingTag);
        return _mapper.Map<TagDto>(updatedTag);
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with ID {id} not found");

        await _tagRepository.DeleteAsync(tag);
    }

    public async Task<IEnumerable<int>> GetExistingTagIdsAsync(IEnumerable<int> ids)
    {
        return await _tagRepository.GetExistingTagIdsAsync(ids);
    }
}