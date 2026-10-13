using System.ComponentModel.DataAnnotations;

namespace GroupHomework66.ViewModels;

public class CreateCourseViewModel
{
    [Display(Name = "Название курса")]
    [Required(ErrorMessage = "Введите название курса.")]
    public string Name { get; set; } = null!;

    [Display(Name = "Организация")]
    [Required(ErrorMessage = "Укажите организацию, проводившую курс.")]
    public string Organization { get; set; } = null!;

    [Display(Name = "Описание курса")]
    public string? Description { get; set; }

    [Display(Name = "Дата начала")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Укажите дату начала курса.")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "Дата окончания")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
}