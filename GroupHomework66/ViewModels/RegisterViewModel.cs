using System.ComponentModel.DataAnnotations;
using GroupHomework66.Enums;

namespace GroupHomework66.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
    
    [Required]
    [Display(Name = "Profile Picture")]
    public string ProfilePicture { get; set; }
    
    [Required]
    [Display(Name = "Role")]
    public ChosenRole Role { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string PasswordConfirm { get; set; }
}