using System.ComponentModel.DataAnnotations;
using GroupHomework66.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GroupHomework66.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [Display(Name = "Username")]
    [Remote("ValidateUserNameOnRegistration", "Validation", ErrorMessage = "Username occupied")]
    public string Username { get; set; }

    [Required]
    [Display(Name = "Phone Number")]
    [Remote("ValidatePhoneNumber", "Validation", ErrorMessage = "Phone Number must contain digits!")]
    public string PhoneNumber { get; set; }
    
    [Display(Name = "Profile Picture")]
    public string? ProfilePicture { get; set; }
    
    [Required]
    [Display(Name = "Role")]
    public ChosenRole Role { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [Remote("ValidatePassword", "Validation", ErrorMessage = "Incorrect password (it must contain at least 8 characters, 1 digit and 1 symbol!)")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string PasswordConfirm { get; set; }
}