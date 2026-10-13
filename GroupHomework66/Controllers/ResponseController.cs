using GroupHomework66.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Controllers;

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
    [Authorize]
    public async Task<IActionResult> CreateResponseAjax(int? resumeId, int? vacancyId)
    {
        if (resumeId != null && vacancyId != null)
        {
            bool responseExists = await _context.Responses.AnyAsync(r => r.ResumeId == resumeId && r.VacancyId == vacancyId);

            if (!responseExists)
            {
                Models.Response response = new Response()
                {
                    ResumeId = resumeId.Value,
                    VacancyId = vacancyId.Value
                };
                await _context.Responses.AddAsync(response);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Отклик уже существует");
        }
        return NotFound();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveResponseAjax(int? resumeId, int? vacancyId)
    {
        if (resumeId != null && vacancyId != null)
        {
            Response? response = await _context.Responses.FirstOrDefaultAsync(r => r.ResumeId == resumeId && r.VacancyId == vacancyId);

            if (response != null)
            {
                _context.Responses.Remove(response);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Отклика не существует");
        }
        return NotFound();
    }

    [HttpPost]
    [Authorize]
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
    [Authorize]
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