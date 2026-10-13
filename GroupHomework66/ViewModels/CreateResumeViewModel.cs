using System.ComponentModel.DataAnnotations;
using GroupHomework66.Enums;

namespace GroupHomework66.ViewModels;

public class CreateResumeViewModel
{
    [Display(Name = "Категория вакансии")]
    [Required(ErrorMessage = "Выберите категорию вакансии.")]
    [EnumDataType(typeof(VacancyCategory), ErrorMessage = "Выберите корректную категорию вакансии.")]
    public VacancyCategory Category { get; set; }

    [Display(Name = "Название резюме")]
    [Required(ErrorMessage = "Введите название резюме.")]
    public string Title { get; set; } = null!;

    [Display(Name = "Ожидаемая заработная плата")]
    [Required(ErrorMessage = "Укажите ожидаемую заработную плату.")]
    [Range(0, 100_000_000, ErrorMessage = "Зарплата должна быть от 0 до 100 000 000.")]
    public decimal ExpectedSalary { get; set; }

    [Display(Name = "Telegram")]
    [Required(ErrorMessage = "Укажите свой Telegram.")]
    public string Telegram { get; set; } = null!;

    [Display(Name = "Электронная почта")]
    [Required(ErrorMessage = "Укажите адрес электронной почты.")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
    public string Email { get; set; } = null!;

    [Display(Name = "Номер телефона")]
    [Required(ErrorMessage = "Укажите номер телефона.")]
    [Phone(ErrorMessage = "Введите корректный номер телефона.")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Ссылка на Facebook")]
    [Url(ErrorMessage = "Введите корректную ссылку на Facebook.")]
    public string? Facebook { get; set; }

    [Display(Name = "Ссылка на LinkedIn")]
    [Url(ErrorMessage = "Введите корректную ссылку на LinkedIn.")]
    public string? LinkedIn { get; set; }
}