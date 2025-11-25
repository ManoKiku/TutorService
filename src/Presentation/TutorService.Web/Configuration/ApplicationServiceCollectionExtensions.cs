using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TutorService.Application.Intefaces;
using TutorService.Application.Mappers;
using TutorService.Application.Services;

namespace TutorService.Web.Configuration;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITutorProfileService, TutorProfileService>();
        services.AddScoped<ITutorPostService, TutorPostService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<TutorMappingProfile>();
            cfg.AddProfile<SubjectMappingProfile>();
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<SubcategoryMappingProfile>();
        });

        return services;
    }
}