namespace GroupHomework66.Models;

public class Education
{
    public int Id { get; set; }
    public int ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
    public string Institution { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}