namespace TutorService.Domain.Entities;

public class StudentTutorRelation
{
    public int Id { get; set; }
    
    public Guid StudentId { get; set; }
    public User Student { get; set; }
    public Guid TutorId { get; set; }
    public TutorProfile Tutor { get; set; }
    public DateTime AddedAt { get; set; }
}