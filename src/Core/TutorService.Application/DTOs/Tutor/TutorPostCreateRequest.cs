namespace TutorService.Application.DTOs.Tutor;

public class TutorPostCreateRequest
{
    public int SubjectId { get; set; }
    public string Description { get; set; }
    public IEnumerable<int>? TagIds { get; set; }
}
