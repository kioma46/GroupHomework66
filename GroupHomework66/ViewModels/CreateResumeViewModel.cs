using System.ComponentModel.DataAnnotations;
using GroupHomework66.Enums;

namespace GroupHomework66.ViewModels;

public class CreateResumeViewModel
{
    [Required]
    public VacancyCategory Category { get; set; }
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public decimal ExpectedSalary { get; set; }
    public string? Telegram { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Facebook { get; set; }
    public string? LinkedIn { get; set; }
}