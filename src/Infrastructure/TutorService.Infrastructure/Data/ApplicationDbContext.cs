namespace TutorService.Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TutorProfile> TutorProfiles { get; set; }
    public DbSet<Category> SubjectCategories { get; set; }
    public DbSet<Subcategory> SubjectSubcategories { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<TutorPost> TutorPosts { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<TutorPostTag> TutorPostTags { get; set; }
    public DbSet<TutorCity> TutorCities { get; set; }
    public DbSet<StudentTutorRelation> StudentTutorRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentTutorRelation>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<TutorCity>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<TutorPostTag>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<StudentTutorRelation>(entity =>
        {
            entity.HasOne(str => str.Tutor)
                .WithMany()
                .HasForeignKey(str => str.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            
            entity.HasOne(str => str.Student)
                .WithMany()
                .HasForeignKey(str => str.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<TutorCity>(entity =>
        {
            entity.HasOne(tc => tc.City)
                .WithMany()
                .HasForeignKey(tc => tc.CityId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(tc => tc.Tutor)
                .WithMany()
                .HasForeignKey(tc => tc.TutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TutorPostTag>(entity =>
        {
            entity.HasOne(tpt => tpt.TutorPost)
                .WithMany()
                .HasForeignKey(tpt => tpt.TutorPostId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(tpt => tpt.Tag)
                .WithMany()
                .HasForeignKey(tpt => tpt.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(c => c.Tutor)
                .WithMany()
                .HasForeignKey(c => c.TutorId)
                .OnDelete(DeleteBehavior.NoAction); 
        });
        
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasOne(m => m.Chat)
                .WithMany()
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<TutorProfile>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<TutorPost>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Assignment>().HasQueryFilter(a => !a.IsDeleted);
        modelBuilder.Entity<Chat>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Message>().HasQueryFilter(m => !m.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(rt => !rt.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
} 