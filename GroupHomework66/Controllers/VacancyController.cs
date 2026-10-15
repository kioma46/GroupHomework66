using System.Security.Claims;
using GroupHomework66.Models;
using GroupHomework66.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Controllers;


public class VacancyController : Controller
{
    private readonly SiteMobileContext _context;

    public VacancyController(SiteMobileContext context)
    {
        _context = context;
    }
    
    [Authorize(Roles = "Company")]
    public IActionResult MyVacancies()
    {
        return RedirectToAction("Index", "Company");
    }

    [HttpGet]
    [Authorize(Roles = "Company")]
    public IActionResult Create()
    {
        return View(new CreateVacancyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Company")]
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
        Vacancy? vacancy;

        if (User.IsInRole("Company"))
        {
            vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v => v.Id == id && v.EmployerId == userId);
        }
        else
        {
            vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(v => v.Id == id && v.IsPublished);

            if (vacancy != null)
            {
                var userResumeIds = await _context.Resumes
                    .Where(r => r.UserId == userId)
                    .Select(r => r.Id)
                    .ToListAsync();

                vacancy.IsRespondedByTheCurrentUser = await _context.Responses
                    .AnyAsync(r => r.VacancyId == id && userResumeIds.Contains(r.ResumeId));

                ViewBag.UserResumes = await _context.Resumes
                    .Where(r => r.UserId == userId)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Title })
                    .ToListAsync();
            }
        }

        if (vacancy == null)
        {
            return NotFound();
        }

        return View(vacancy);
    }

    [HttpGet]
    [Authorize(Roles = "Company")]
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
    [Authorize(Roles = "Company")]
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
    [Authorize(Roles = "Company")]
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
    [Authorize(Roles = "Company")]
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

    [HttpGet]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> List()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var vacancies = await _context.Vacancies.ToListAsync();

        var userResumeIds = await _context.Resumes
            .Where(r => r.UserId == userId)
            .Select(r => r.Id)
            .ToListAsync();

        var respondedVacancyIds = await _context.Responses
            .Where(r => userResumeIds.Contains(r.ResumeId))
            .Select(r => r.VacancyId)
            .ToListAsync();

        foreach (var vacancy in vacancies)
        {
            vacancy.IsRespondedByTheCurrentUser = respondedVacancyIds.Contains(vacancy.Id);
        }

        ViewBag.UserResumes = await _context.Resumes
            .Where(r => r.UserId == userId)
            .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Title })
            .ToListAsync();

        return View(vacancies);
    }
}