namespace TutorService.Domain.Entities;

public class Subcategory
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public string Name { get; set; }

    public ICollection<Subject> Subjects { get; set; }
}