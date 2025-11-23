namespace TutorService.Domain.Entities;

public class TutorCity
{
    public Guid TutorId { get; set; }
    public TutorProfile Tutor { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
}