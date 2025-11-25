using TutorService.Domain.Enums;

namespace TutorService.Application.DTOs.Tutor;

public class TutorPostDto
{
    public Guid Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public Guid TutorId { get; set; }
    public string TutorName { get; set; }
    public string Description { get; set; }
    public decimal HourlyRate { get; set; }
    public PostStatus Status { get; set; }
    public string? AdminComment { get; set; }
    public IEnumerable<TagDto> Tags { get; set; }
}
