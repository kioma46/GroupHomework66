using System.Security.Claims;
using GroupHomework66.Models;
using GroupHomework66.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Controllers;

[Authorize(Roles = "Applicant")]
public class CandidateController : Controller
{
    private readonly SiteMobileContext _context;

    public CandidateController(SiteMobileContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resumes = await _context.Resumes.Where(r => r.UserId == userId).ToListAsync();

        return View(resumes);
    }
    
    [HttpGet]
    public IActionResult CreateResume()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateResume(CreateResumeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resume = new Resume
        {
            UserId = userId,
            Category = model.Category,
            Title = model.Title,
            ExpectedSalary = model.ExpectedSalary,
            Telegram = model.Telegram,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Facebook = model.Facebook,
            LinkedIn = model.LinkedIn
        };

        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> EditResume(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (resume == null)
        {
            return NotFound();
        }

        var model = new CreateResumeViewModel
        {
            Category = resume.Category,
            Title = resume.Title,
            ExpectedSalary = resume.ExpectedSalary,
            Telegram = resume.Telegram,
            Email = resume.Email,
            PhoneNumber = resume.PhoneNumber,
            Facebook = resume.Facebook,
            LinkedIn = resume.LinkedIn
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditResume(int id, CreateResumeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (resume == null)
        {
            return NotFound();
        }

        resume.Category = model.Category;
        resume.Title = model.Title;
        resume.ExpectedSalary = model.ExpectedSalary;
        resume.Telegram = model.Telegram;
        resume.Email = model.Email;
        resume.PhoneNumber = model.PhoneNumber;
        resume.Facebook = model.Facebook;
        resume.LinkedIn = model.LinkedIn;
        resume.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resume = await _context.Resumes.Include(r => r.WorkExperiences).FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (resume == null)
        {
            return NotFound();
        }

        return View(resume);
    }
    
    [HttpGet]
    public async Task<IActionResult> AddWorkExperience(int resumeId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resumeExists = await _context.Resumes.AnyAsync(r => r.Id == resumeId && r.UserId == userId);
        if (!resumeExists)
        {
            return NotFound();
        }

        ViewBag.ResumeId = resumeId;

        return View(new CreateWorkExperienceViewModel());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWorkExperience(int resumeId, CreateWorkExperienceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = resumeId;
            return View(model);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resumeExists = await _context.Resumes.AnyAsync(r => r.Id == resumeId && r.UserId == userId);
        if (!resumeExists)
        {
            return NotFound();
        }

        var workExperience = new WorkExperience
        {
            ResumeId = resumeId,
            CompanyName = model.CompanyName,
            Position = model.Position,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        _context.WorkExperiences.Add(workExperience);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }
}