using TutorService.Domain.Enums;

namespace TutorService.Domain.Entities;

public class Lesson : BaseEntity
{
    public Guid TutorId { get; set; }
    public TutorProfile Tutor { get; set; }
    public Guid StudentId { get; set; }
    public User Student { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Title { get; set; }
    public LessonStatus Status { get; set; }

    public ICollection<Assignment> Assignments { get; set; }
}
