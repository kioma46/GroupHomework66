using System.Security.Claims;
using GroupHomework66.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupHomework66.ViewModels;

namespace GroupHomework66.Controllers;

[Authorize]
public class ResponseController : Controller
{
    private readonly SiteMobileContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public ResponseController(SiteMobileContext context, UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    
    [HttpPost]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Create(int vacancyId, int resumeId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var ownsResume = await _context.Resumes.AnyAsync(r => r.Id == resumeId && r.UserId == userId);
        if (!ownsResume)
            return BadRequest("Резюме не найдено или не принадлежит вам.");

        var userResumeIds = await _context.Resumes
            .Where(r => r.UserId == userId)
            .Select(r => r.Id)
            .ToListAsync();

        bool responseExists = await _context.Responses
            .AnyAsync(r => r.VacancyId == vacancyId && userResumeIds.Contains(r.ResumeId));
    
        if (responseExists)
            return BadRequest("Вы уже откликнулись на эту вакансию.");

        var response = new Response
        {
            VacancyId = vacancyId,
            ResumeId = resumeId,
            ResponseDate = DateTime.UtcNow
        };

        await _context.Responses.AddAsync(response);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> Remove(int vacancyId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var userResumeIds = await _context.Resumes
            .Where(r => r.UserId == userId)
            .Select(r => r.Id)
            .ToListAsync();

        if (!userResumeIds.Any())
        {
            return NotFound("У вас нет резюме.");
        }

        var response = await _context.Responses
            .FirstOrDefaultAsync(r => r.VacancyId == vacancyId && userResumeIds.Contains(r.ResumeId));

        if (response == null)
        {
            return NotFound("Отклик не найден.");
        }

        _context.Responses.Remove(response);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> GetResponsesForCompanyAjax(int? companyId)
    {
        if (!companyId.HasValue)
            return BadRequest("Company ID не передан");

        var responses = await _context.Responses
            .Where(r => r.Vacancy != null && r.Vacancy.EmployerId == companyId.Value)
            .Include(r => r.Resume)
            .Include(r => r.Vacancy)
            .Select(r => new 
            {
                r.Id,
                r.ResponseDate,
                VacancyId = r.VacancyId,
                VacancyTitle = r.Vacancy != null ? r.Vacancy.Title : null,
                ResumeId = r.ResumeId
            })
            .ToListAsync();

        return Json(responses);
    }
    
    [HttpPost]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> GetResponsesForApplicantAjax(int? applicantId)
    {
        if (!applicantId.HasValue)
            return BadRequest("ApplicantID не передан");
    
        var responses = await _context.Responses
            .Where(r => r.Resume != null && r.Resume.UserId == applicantId.Value)
            .Include(r => r.Resume)
            .Include(r => r.Vacancy)
            .Select(r => new 
            {
                r.Id,
                r.ResponseDate,
                ResumeId = r.ResumeId,
                ResumeTitle = r.Resume != null ? r.Resume.Title : null,
                VacancyId = r.VacancyId,
                VacancyTitle = r.Vacancy != null ? r.Vacancy.Title : null
            })
            .ToListAsync();

        return Json(responses);
    }
    
    
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CompanyResponses()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var vacancies = await _context.Vacancies
            .Where(v => v.EmployerId == userId)
            .OrderByDescending(v => v.UpdatedDate)
            .ToListAsync();

        var responses = await _context.Responses
            .Where(r => r.Vacancy!.EmployerId == userId)
            .Include(r => r.Resume).ThenInclude(r => r!.User)
            .OrderByDescending(r => r.ResponseDate)
            .ToListAsync();

        var model = new CompanyResponsesViewModel
        {
            Vacancies = vacancies.Select(v => new VacancyResponsesGroup
            {
                VacancyId = v.Id,
                VacancyTitle = v.Title,
                IsPublished = v.IsPublished,
                Responses = responses
                    .Where(r => r.VacancyId == v.Id)
                    .Select(r => new ResponseItem
                    {
                        ResponseId = r.Id,
                        ResumeTitle = r.Resume?.Title ?? "Резюме удалено",
                        ApplicantName = r.Resume?.User?.UserName ?? "Неизвестно",
                        ResponseDate = r.ResponseDate
                    })
                    .ToList()
            }).ToList()
        };

        return View(model);
    }
    
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> ViewResume(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var response = await _context.Responses
            .Include(r => r.Vacancy)
            .Include(r => r.Resume).ThenInclude(r => r!.User)
            .Include(r => r.Resume).ThenInclude(r => r!.WorkExperiences)
            .Include(r => r.Resume).ThenInclude(r => r!.Educations)
            .Include(r => r.Resume).ThenInclude(r => r!.Courses)
            .FirstOrDefaultAsync(r => r.Id == id && r.Vacancy!.EmployerId == userId);

        if (response == null || response.Resume == null)
        {
            return NotFound();
        }

        return View(response);
    }
        
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> ApplicantResponses()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var responses = await _context.Responses
            .Where(r => r.Resume != null && r.Resume.UserId == userId)
            .Include(r => r.Resume)
            .Include(r => r.Vacancy)
            .ThenInclude(v => v!.Employer)
            .OrderByDescending(r => r.ResponseDate)
            .ToListAsync();

        return View(responses);
    }
}