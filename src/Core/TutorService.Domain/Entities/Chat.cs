namespace TutorService.Domain.Entities;

public class Chat : BaseEntity
{
    public Guid TutorId { get; set; }
    public TutorProfile Tutor { get; set; }
    public Guid StudentId { get; set; }
    public User Student { get; set; }

    public ICollection<Message> Messages { get; set; }
}