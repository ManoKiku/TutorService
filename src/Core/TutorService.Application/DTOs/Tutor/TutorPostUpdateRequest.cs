namespace TutorService.Application.DTOs.Tutor;

public class TutorPostUpdateRequest
{
    public int SubjectId { get; set; }
    public string Description { get; set; }
    public decimal HourlyRate { get; set; }
}
