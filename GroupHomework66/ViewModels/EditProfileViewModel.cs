using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GroupHomework66.ViewModels;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Введите адрес электронной почты.")]
    [Display(Name = "Адрес электронной почты")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Введите имя пользователя.")]
    [Display(Name = "Имя пользователя")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Введите номер телефона.")]
    [Display(Name = "Номер телефона")]
    [Remote("ValidatePhoneNumber", "Validation", ErrorMessage = "Неверный формат номера.")]
    public string PhoneNumber { get; set; }
    
    [Display(Name = "Фото профиля")]
    public string? ProfilePicture { get; set; }
}