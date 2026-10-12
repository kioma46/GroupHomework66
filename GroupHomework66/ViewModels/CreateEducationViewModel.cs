namespace GroupHomework66.ViewModels;

public class CreateEducationViewModel
{
    public string Institution { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}