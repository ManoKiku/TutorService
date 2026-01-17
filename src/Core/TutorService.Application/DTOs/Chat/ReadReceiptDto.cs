namespace TutorService.Application.DTOs.Chat;

public class ReadReceiptDto
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }
}