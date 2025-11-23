using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorService.Domain.Entities;

namespace TutorService.Infrastructure.Data.Configurations;

public class TutorPostConfiguration : BaseEntityConfiguration<TutorPost>
{
    public override void Configure(EntityTypeBuilder<TutorPost> builder)
    {
        base.Configure(builder);

        builder.ToTable("TutorPosts");
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(p => p.Tutor)
            .WithMany(t => t.TutorPosts)
            .HasForeignKey(p => p.TutorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Subject)
            .WithMany(s => s.TutorPosts)
            .HasForeignKey(p => p.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.TutorPostTags)
            .WithOne(pt => pt.TutorPost)
            .HasForeignKey(pt => pt.TutorPostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}