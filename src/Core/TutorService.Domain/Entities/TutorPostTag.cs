namespace TutorService.Domain.Entities;

public class TutorPostTag
{
    public int Id { get; set; }
    public Guid TutorPostId { get; set; }
    public TutorPost TutorPost { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}