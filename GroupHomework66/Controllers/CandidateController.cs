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
        var resume = await _context.Resumes
            .Include(r => r.WorkExperiences)
            .Include(r => r.Courses)
            .Include(r => r.Educations)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
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
            StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc),
            EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null
        };

        _context.WorkExperiences.Add(workExperience);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditWorkExperience(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var experience = await _context.WorkExperiences.Include(w => w.Resume)
            .FirstOrDefaultAsync(w => w.Id == id && w.Resume.UserId == userId);
        if (experience == null)
        {
            return NotFound();
        }

        ViewBag.ResumeId = experience.ResumeId;

        var model = new CreateWorkExperienceViewModel
        {
            CompanyName = experience.CompanyName,
            Position = experience.Position,
            Description = experience.Description,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditWorkExperience(int id, CreateWorkExperienceViewModel model)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var experience = await _context.WorkExperiences.Include(w => w.Resume)
            .FirstOrDefaultAsync(w => w.Id == id && w.Resume.UserId == userId);
        if (experience == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = experience.ResumeId;
            return View(model);
        }

        experience.CompanyName = model.CompanyName;
        experience.Position = model.Position;
        experience.Description = model.Description;
        experience.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
        experience.EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = experience.ResumeId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteWorkExperience(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var experience = await _context.WorkExperiences.Include(w => w.Resume)
            .FirstOrDefaultAsync(w => w.Id == id && w.Resume.UserId == userId);
        if (experience == null)
            return NotFound();

        var resumeId = experience.ResumeId;
        _context.WorkExperiences.Remove(experience);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> AddCourse(int resumeId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resumeExists = await _context.Resumes.AnyAsync(r => r.Id == resumeId && r.UserId == userId);
        if (!resumeExists)
        {
            return NotFound();
        }

        ViewBag.ResumeId = resumeId;

        return View(new CreateCourseViewModel());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCourse(int resumeId, CreateCourseViewModel model)
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

        var course = new Course
        {
            ResumeId = resumeId,
            Name = model.Name,
            Organization = model.Organization,
            Description = model.Description,
            StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc),
            EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditCourse(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var course = await _context.Courses.Include(c => c.Resume)
            .FirstOrDefaultAsync(c => c.Id == id && c.Resume.UserId == userId);
        if (course == null)
        {
            return NotFound();
        }

        ViewBag.ResumeId = course.ResumeId;

        var model = new CreateCourseViewModel
        {
            Name = course.Name,
            Organization = course.Organization,
            Description = course.Description,
            StartDate = course.StartDate,
            EndDate = course.EndDate
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(int id, CreateCourseViewModel model)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var course = await _context.Courses.Include(c => c.Resume)
            .FirstOrDefaultAsync(c => c.Id == id && c.Resume.UserId == userId);
        if (course == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = course.ResumeId;
            return View(model);
        }

        course.Name = model.Name;
        course.Organization = model.Organization;
        course.Description = model.Description;
        course.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
        course.EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = course.ResumeId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var course = await _context.Courses.Include(c => c.Resume)
            .FirstOrDefaultAsync(c => c.Id == id && c.Resume.UserId == userId);
        if (course == null)
            return NotFound();

        var resumeId = course.ResumeId;
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> AddEducation(int resumeId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resumeExists = await _context.Resumes.AnyAsync(r => r.Id == resumeId && r.UserId == userId);
        if (!resumeExists)
        {
            return NotFound();
        }

        ViewBag.ResumeId = resumeId;

        return View(new CreateEducationViewModel());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEducation(int resumeId, CreateEducationViewModel model)
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

        var education = new Education
        {
            ResumeId = resumeId,
            Institution = model.Institution,
            Specialty = model.Specialty,
            Description = model.Description,
            StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc),
            EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null
        };

        _context.Educations.Add(education);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditEducation(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var education = await _context.Educations.Include(e => e.Resume)
            .FirstOrDefaultAsync(e => e.Id == id && e.Resume.UserId == userId);
        if (education == null)
        {
            return NotFound();
        }

        ViewBag.ResumeId = education.ResumeId;

        var model = new CreateEducationViewModel
        {
            Institution = education.Institution,
            Specialty = education.Specialty,
            Description = education.Description,
            StartDate = education.StartDate,
            EndDate = education.EndDate
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEducation(int id, CreateEducationViewModel model)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var education = await _context.Educations.Include(e => e.Resume)
            .FirstOrDefaultAsync(e => e.Id == id && e.Resume.UserId == userId);
        if (education == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = education.ResumeId;
            return View(model);
        }

        education.Institution = model.Institution;
        education.Specialty = model.Specialty;
        education.Description = model.Description;
        education.StartDate = DateTime.SpecifyKind(model.StartDate, DateTimeKind.Utc);
        education.EndDate = model.EndDate.HasValue ? DateTime.SpecifyKind(model.EndDate.Value, DateTimeKind.Utc) : null;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = education.ResumeId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEducation(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var education = await _context.Educations.Include(e => e.Resume)
            .FirstOrDefaultAsync(e => e.Id == id && e.Resume.UserId == userId);
        if (education == null)
            return NotFound();

        var resumeId = education.ResumeId;
        _context.Educations.Remove(education);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = resumeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id, string? returnUrl)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (resume == null)
            return NotFound();

        resume.IsPublished = !resume.IsPublished;
        await _context.SaveChangesAsync();
        
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (resume == null)
            return NotFound();

        resume.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index", "Candidate");
    }
}