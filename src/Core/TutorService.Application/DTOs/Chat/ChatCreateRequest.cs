namespace TutorService.Application.DTOs.Chat;

public class ChatCreateRequest
{
    public Guid TutorId { get; set; }
    public Guid StudentId { get; set; }
}