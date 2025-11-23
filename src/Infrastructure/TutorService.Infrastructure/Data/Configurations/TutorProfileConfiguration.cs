namespace TutorService.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TutorProfileConfiguration : BaseEntityConfiguration<TutorProfile>
{
    public override void Configure(EntityTypeBuilder<TutorProfile> builder)
    {
        base.Configure(builder);

        builder.ToTable("TutorProfiles");

        builder.Property(t => t.Bio)
            .HasMaxLength(1000);

        builder.Property(t => t.Education)
            .HasMaxLength(500);

        builder.Property(t => t.ExperienceYears)
            .IsRequired();

        builder.Property(t => t.HourlyRate)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.HasOne(t => t.User)
            .WithMany(u => u.TutorProfiles)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.TutorPosts)
            .WithOne(p => p.Tutor)
            .HasForeignKey(p => p.TutorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Lessons)
            .WithOne(l => l.Tutor)
            .HasForeignKey(l => l.TutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
