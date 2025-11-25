using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorService.Domain.Entities;

namespace TutorService.Infrastructure.Data.Configurations;

public class StudentTutorRelationConfiguration : IEntityTypeConfiguration<StudentTutorRelation>
{
    public void Configure(EntityTypeBuilder<StudentTutorRelation> builder)
    {
        builder.HasOne(e => e.Student)
            .WithMany() 
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Tutor)
            .WithMany()
            .HasForeignKey(e => e.TutorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(e => new { e.StudentId, e.TutorId })
            .IsUnique();
    }
}