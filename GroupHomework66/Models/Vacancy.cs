using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupHomework66.Enums;

namespace GroupHomework66.Models
{
    public class Vacancy : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        public int EmployerId { get; set; }

        [ForeignKey(nameof(EmployerId))]
        public User? Employer { get; set; }

        [Required(ErrorMessage = "Название вакансии обязательно")]
        [Display(Name = "Название вакансии")]
        [StringLength(150, ErrorMessage = "Длина названия не должна превышать 150 символов")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите заработную плату")]
        [Range(0, 100_000_000, ErrorMessage = "Некорректная сумма")]
        [Display(Name = "Заработная плата")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Детальное описание обязательно")]
        [StringLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
        [Display(Name = "Детальное описание и требования")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Требуемый опыт от (лет)")]
        [Range(0, 50, ErrorMessage = "Опыт должен быть от 0 до 50 лет")]
        public int ExperienceFrom { get; set; }

        [Display(Name = "Требуемый опыт до (лет)")]
        [Range(0, 50, ErrorMessage = "Опыт должен быть от 0 до 50 лет")]
        public int ExperienceTo { get; set; }

        [Required(ErrorMessage = "Выберите категорию вакансии")]
        [Display(Name = "Категория")]
        public VacancyCategory Category { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Дата обновления")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Опубликовано")]
        public bool IsPublished { get; set; } = true;

        // public ICollection<Response> Responses { get; set; } = new List<Response>();

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (ExperienceTo < ExperienceFrom)
                yield return new ValidationResult(
                    "«Опыт до» не может быть меньше «Опыт от»",
                    new[] { nameof(ExperienceTo) });
        }
    }
}