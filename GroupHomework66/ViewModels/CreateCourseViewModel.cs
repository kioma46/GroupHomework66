using System.ComponentModel.DataAnnotations;

namespace GroupHomework66.ViewModels;

public class CreateCourseViewModel
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Organization { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}