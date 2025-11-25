using TutorService.Domain.Enums;

namespace TutorService.Domain.Entities;

public class TutorPost : BaseEntity
{
    public Guid TutorId { get; set; }
    public TutorProfile Tutor { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    public string Description { get; set; }
    public PostStatus Status { get; set; }
    public ICollection<TutorPostTag> TutorPostTags { get; set; }
}