using System.Security.Claims;
using GroupHomework66.Models;
using GroupHomework66.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Controllers;

[Authorize(Roles = "Company")]
public class VacancyController : Controller
{
    private readonly SiteMobileContext _context;

    public VacancyController(SiteMobileContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> MyVacancies()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancies = await _context.Vacancies
            .Where(v => v.EmployerId == userId)
            .OrderByDescending(v => v.UpdatedDate)
            .ToListAsync();

        return View(vacancies);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateVacancyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVacancyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var now = DateTime.UtcNow;

        var vacancy = new Vacancy
        {
            EmployerId = userId,
            Title = model.Title.Trim(),
            Salary = model.Salary!.Value,
            Description = model.Description.Trim(),
            ExperienceFrom = model.ExperienceFrom,
            ExperienceTo = model.ExperienceTo,
            Category = model.Category!.Value,
            CreatedDate = now,
            UpdatedDate = now,
            IsPublished = true
        };

        _context.Vacancies.Add(vacancy);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyVacancies));
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == userId);
        if (vacancy == null)
        {
            return NotFound();
        }

        return View(vacancy);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == userId);
        if (vacancy == null)
        {
            return NotFound();
        }

        var model = new CreateVacancyViewModel
        {
            Title = vacancy.Title,
            Salary = vacancy.Salary,
            Description = vacancy.Description,
            ExperienceFrom = vacancy.ExperienceFrom,
            ExperienceTo = vacancy.ExperienceTo,
            Category = vacancy.Category
        };

        ViewBag.VacancyId = id;
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateVacancyViewModel model)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == userId);
        if (vacancy == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.VacancyId = id;
            return View(model);
        }

        vacancy.Title = model.Title.Trim();
        vacancy.Salary = model.Salary!.Value;
        vacancy.Description = model.Description.Trim();
        vacancy.ExperienceFrom = model.ExperienceFrom;
        vacancy.ExperienceTo = model.ExperienceTo;
        vacancy.Category = model.Category!.Value;
        vacancy.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyVacancies));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == userId);
        if (vacancy == null)
        {
            return NotFound();
        }

        vacancy.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyVacancies));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == userId);
        if (vacancy == null)
        {
            return NotFound();
        }

        vacancy.IsPublished = !vacancy.IsPublished;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyVacancies));
    }
}