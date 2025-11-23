using TutorService.Domain.Enums;

namespace TutorService.Domain.Entities;

public class User : BaseEntity
{
    public UserRole Role { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public bool IsEmailVerified { get; set; }

    public ICollection<TutorProfile> TutorProfiles { get; set; }
    public ICollection<Lesson> LessonsAsStudent { get; set; }
    public ICollection<Chat> ChatsAsStudent { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<StudentTutorRelation> StudentRelations { get; set; }
    public ICollection<StudentTutorRelation> TutorRelations { get; set; }
}