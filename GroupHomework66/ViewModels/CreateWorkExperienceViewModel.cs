using System.ComponentModel.DataAnnotations;

namespace GroupHomework66.ViewModels;

public class CreateWorkExperienceViewModel
{
    [Display(Name = "Компания")]
    [Required(ErrorMessage = "Укажите название компании.")]
    public string CompanyName { get; set; } = null!;

    [Display(Name = "Должность")]
    [Required(ErrorMessage = "Укажите занимаемую должность.")]
    public string Position { get; set; } = null!;

    [Display(Name = "Описание обязанностей")]
    public string? Description { get; set; }

    [Display(Name = "Дата начала работы")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Укажите дату начала работы.")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "Дата окончания работы")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
}