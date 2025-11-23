using Microsoft.Extensions.DependencyInjection;
using TutorService.Application.Intefaces;
using TutorService.Application.Services;

namespace TutorService.Web.Configuration;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}