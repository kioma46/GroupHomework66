using System.Security.Claims;
using GroupHomework66.Models;
using GroupHomework66.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Controllers;

[Authorize(Roles = "Company")]
public class CompanyController : Controller
{
    private const string DefaultPicture = "https://dme.lk/images/user.jpg";

    private readonly SiteMobileContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public CompanyController(SiteMobileContext context, UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        return View(await BuildModel(user, null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditCompanyProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            user.UserName = model.CompanyName.Trim();
            user.Email = model.Email.Trim();
            user.PhoneNumber = model.PhoneNumber.Trim();
            user.ProfilePicture = string.IsNullOrWhiteSpace(model.ProfilePicture)
                ? DefaultPicture
                : model.ProfilePicture.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        
        user = await _userManager.GetUserAsync(User);
        ViewBag.OpenModal = true;
        return View("Index", await BuildModel(user!, model));
    }

    private async Task<CompanyProfileViewModel> BuildModel(User user, EditCompanyProfileViewModel? edit)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancies = await _context.Vacancies
            .Where(v => v.EmployerId == userId)
            .OrderByDescending(v => v.UpdatedDate)
            .ToListAsync();

        return new CompanyProfileViewModel
        {
            CompanyName = user.UserName ?? string.Empty,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            Vacancies = vacancies,
            Edit = edit ?? new EditCompanyProfileViewModel
            {
                CompanyName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ProfilePicture = user.ProfilePicture
            }
        };
    }
}