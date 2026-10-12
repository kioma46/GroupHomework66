namespace GroupHomework66.Models;

public class WorkExperience
{
    public int Id { get; set; }
    public int ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}