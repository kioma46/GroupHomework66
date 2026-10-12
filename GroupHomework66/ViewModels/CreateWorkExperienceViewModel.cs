using System.ComponentModel.DataAnnotations;

namespace GroupHomework66.ViewModels;

public class CreateWorkExperienceViewModel
{
    [Required]
    public string CompanyName { get; set; } = null!;
    [Required]
    public string Position { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}