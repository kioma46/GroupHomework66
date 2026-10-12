namespace GroupHomework66.Models;

public class Course
{
    public int Id { get; set; }
    public int ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Organization { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}