using TutorService.Domain.Entities;

public class TutorProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Bio { get; set; }
    public string Education { get; set; }
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }

    public ICollection<TutorPost> TutorPosts { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
    public ICollection<Chat> Chats { get; set; }
    public ICollection<TutorCity> TutorCities { get; set; }
    public ICollection<StudentTutorRelation> StudentTutorRelations { get; set; }
}