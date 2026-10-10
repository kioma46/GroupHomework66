using GroupHomework66.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupHomework66.Controllers;

public class ValidationController : Controller
{
    
    private SiteMobileContext _context;
    private readonly UserManager<User> _userManager;

    public ValidationController(UserManager<User> userManager, SiteMobileContext context)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public bool ValidatePhoneNumber(string PhoneNumber) => PhoneNumber.All(char.IsDigit);
    public bool ValidateEmailOnRegistration(string Email) => !_userManager.Users.Any(u => u.Email == Email);
    public bool ValidateUserNameOnRegistration(string UserName) => !_userManager.Users.Any(u => u.UserName == UserName);
    public bool ValidatePassword(string Password) => Password.Length >= 8 && Password.Any(char.IsDigit) && Password.Any(char.IsUpper) && Password.Any(c => !char.IsLetter(c));

    public bool ValidateDateOfBirth(DateTime DateOfBirth) => DateOfBirth <= DateTime.UtcNow.AddYears(-18);
}