using System.ComponentModel.DataAnnotations;

namespace GroupHomework66.Enums;

public enum VacancyCategory
{
    [Display(Name = "IT и разработка")] It = 1,
    [Display(Name = "Маркетинг")] Marketing,
    [Display(Name = "Продажи")] Sales,
    [Display(Name = "Финансы")] Finance,
    [Display(Name = "Дизайн")] Design,
    [Display(Name = "Образование")] Education,
    [Display(Name = "Другое")] Other
}