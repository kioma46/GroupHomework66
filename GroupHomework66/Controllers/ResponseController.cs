using System.Security.Claims;
using GroupHomework66.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    
    
    
    
}