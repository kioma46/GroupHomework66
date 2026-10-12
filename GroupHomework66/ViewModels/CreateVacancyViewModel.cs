using System.ComponentModel.DataAnnotations;
using GroupHomework66.Enums;

namespace GroupHomework66.ViewModels
{
    public class CreateVacancyViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Название вакансии обязательно")]
        [StringLength(150, ErrorMessage = "Длина названия не должна превышать 150 символов")]
        [Display(Name = "Название вакансии")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите заработную плату")]
        [Range(0, 100_000_000, ErrorMessage = "Некорректная сумма")]
        [Display(Name = "Заработная плата")]
        public decimal? Salary { get; set; }

        [Required(ErrorMessage = "Детальное описание обязательно")]
        [StringLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
        [Display(Name = "Детальное описание и требования")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 50, ErrorMessage = "Опыт должен быть от 0 до 50 лет")]
        [Display(Name = "Требуемый опыт от (лет)")]
        public int ExperienceFrom { get; set; }

        [Range(0, 50, ErrorMessage = "Опыт должен быть от 0 до 50 лет")]
        [Display(Name = "Требуемый опыт до (лет)")]
        public int ExperienceTo { get; set; }

        [Required(ErrorMessage = "Выберите категорию вакансии")]
        [Display(Name = "Категория")]
        public VacancyCategory? Category { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (ExperienceTo < ExperienceFrom)
                yield return new ValidationResult(
                    "«Опыт до» не может быть меньше «Опыт от»",
                    new[] { nameof(ExperienceTo) });
        }
    }
}