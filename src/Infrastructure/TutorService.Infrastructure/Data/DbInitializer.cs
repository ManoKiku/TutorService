using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TutorService.Domain.Entities;

namespace TutorService.Infrastructure.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(ApplicationDbContext context, ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedDefaultDataAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedDefaultDataAsync()
    {
        if (!await _context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "Школа" },
                new() { Name = "Университет" },
                new() { Name = "Колледж/ПТУ" },
                new() { Name = "Хобби" },
                new() { Name = "Профессиональные навыки" }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            var schoolCategory = categories.First(c => c.Name == "Школа");
            var schoolSubcategories = new List<Subcategory>
            {
                new() { CategoryId = schoolCategory.Id, Name = "1-4 класс" },
                new() { CategoryId = schoolCategory.Id, Name = "5-9 класс" },
                new() { CategoryId = schoolCategory.Id, Name = "10-11 класс" },
                new() { CategoryId = schoolCategory.Id, Name = "Подготовка к ЦТ/ЦЭ" },
                new() { CategoryId = schoolCategory.Id, Name = "Экзамены 9 класс" }
            };

            await _context.Subcategories.AddRangeAsync(schoolSubcategories);
            await _context.SaveChangesAsync();

            var tags = new List<Tag>
            {
                new() { Name = "Онлайн" },
                new() { Name = "Выезд" },
                new() { Name = "У себя" },
                new() { Name = "Экзамены" },
                new() { Name = "Подготовка к школе" },
                new() { Name = "Университетская программа" }
            };

            await _context.Tags.AddRangeAsync(tags);
            await _context.SaveChangesAsync();

            var cities = new List<City>
            {
                new() { Name = "Минск" },
                new() { Name = "Брест" },
                new() { Name = "Могилев" },
                new() { Name = "Гродно" },
                new() { Name = "Витебск" },
                new() { Name = "Гомель" }, 
            };

            await _context.Cities.AddRangeAsync(cities);
            await _context.SaveChangesAsync();
        }
    }
}