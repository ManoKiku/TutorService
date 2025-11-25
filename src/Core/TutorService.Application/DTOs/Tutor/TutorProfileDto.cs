namespace TutorService.Application.DTOs.Tutor;

public class TutorProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Bio { get; set; }
    public string Education { get; set; }
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
}
