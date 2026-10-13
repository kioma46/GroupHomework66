using System.ComponentModel.DataAnnotations;
using GroupHomework66.Models;

namespace GroupHomework66.ViewModels;

public class EditCompanyProfileViewModel
{
    [Required(ErrorMessage = "Укажите имя компании")]
    [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
    [Display(Name = "Имя компании")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите номер телефона")]
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    [Display(Name = "Номер телефона")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Url(ErrorMessage = "Укажите корректную ссылку на изображение")]
    [Display(Name = "Ссылка на фото профиля")]
    public string? ProfilePicture { get; set; }
}

public class CompanyProfileViewModel
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }

    public EditCompanyProfileViewModel Edit { get; set; } = new();
    public List<Vacancy> Vacancies { get; set; } = new();
}