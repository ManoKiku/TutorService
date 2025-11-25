namespace TutorService.Application.DTOs.Tutor;

public class TutorProfileUpdateRequest
{
    public string Bio { get; set; }
    public string Education { get; set; }
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
}
