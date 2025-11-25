using AutoMapper;
using TutorService.Application.DTOs.Category;
using TutorService.Application.DTOs.Subcategory;
using TutorService.Application.DTOs.Tutor;
using TutorService.Domain.Entities;

namespace TutorService.Application.Mappers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
    }
}