using System.ComponentModel.DataAnnotations;
using GroupHomework66.Enums;

namespace GroupHomework66.Models;

public class Resume
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
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
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; }
    public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
    public ICollection<Education> Educations { get; set; } = new List<Education>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}