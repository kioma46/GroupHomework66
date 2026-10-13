using System.ComponentModel.DataAnnotations;

namespace GroupHomework66.ViewModels;

public class CreateEducationViewModel
{
    [Display(Name = "Учебное заведение")]
    [Required(ErrorMessage = "Укажите название учебного заведения.")]
    public string Institution { get; set; } = null!;

    [Display(Name = "Специальность")]
    [Required(ErrorMessage = "Укажите специальность.")]
    public string Specialty { get; set; } = null!;

    [Display(Name = "Дополнительная информация")]
    public string? Description { get; set; }

    [Display(Name = "Дата начала обучения")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Укажите дату начала обучения.")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "Дата окончания обучения")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
}