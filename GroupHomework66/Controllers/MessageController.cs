using System.Security.Claims;
using GroupHomework66.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Controllers;

[Authorize]
public class MessageController : Controller
{
    private readonly SiteMobileContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    
    public MessageController(SiteMobileContext context, UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(int candidateId, int employerId)
    {
        var currentUserId = CurrentUserId;

        if (currentUserId != candidateId && currentUserId != employerId)
            return Forbid();

        bool hasResponse = await _context.Responses
            .AnyAsync(r => r.Resume != null && r.Resume.UserId == candidateId && 
                           r.Vacancy != null && r.Vacancy.EmployerId == employerId);

        if (!hasResponse)
            return BadRequest("Доступ к чату ограничен: отсутствует отклик на вакансию.");

        ViewBag.CandidateId = candidateId;
        ViewBag.EmployerId = employerId;

        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMessageAjax(int? candidateId, int? employerId)
    {
        if (!candidateId.HasValue || !employerId.HasValue)
            return BadRequest("Не переданы необходимые идентификаторы.");

        int currentUserId = CurrentUserId;

        if (currentUserId != candidateId && currentUserId != employerId)
            return Forbid();

        bool hasResponse = await _context.Responses
            .Include(r => r.Resume)
            .Include(r => r.Vacancy)
            .AnyAsync(r => r.Resume != null && r.Resume.UserId == candidateId && 
                           r.Vacancy != null && r.Vacancy.EmployerId == employerId);

        if (!hasResponse)
            return BadRequest("Доступ к чату ограничен: отсутствует отклик на вакансию.");
        
        var messages = await _context.Set<Message>()
            .Where(m => (m.SenderUserId == candidateId && m.ReceiverUserId == employerId) ||
                        (m.SenderUserId == employerId && m.ReceiverUserId == candidateId))
            .OrderBy(m => m.CreatedAt).Include(m => m.User).Select(m => new
            {
                m.Id,
                m.MessageText,
                m.CreatedAt,
                senderUserName = m.User.UserName,
                m.SenderUserId,
                m.ReceiverUserId
            })
            .ToListAsync();

        return Json(messages);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessageAjax(int employerId, string messageText, int? candidateId)
    {
        if (string.IsNullOrWhiteSpace(messageText))
            return BadRequest("Сообщение не может быть пустым.");
        
        var currentUserId = CurrentUserId;

        if (currentUserId == employerId)
            return BadRequest("Нельзя отправить сообщение самому себе.");

        var r = await _context.Responses
            .Include(r => r.Resume).Include(r => r.Vacancy).FirstOrDefaultAsync(r => 
                (r.Resume != null && r.Resume.UserId == currentUserId && r.Vacancy != null && r.Vacancy.EmployerId == employerId) ||
                (r.Resume != null && r.Resume.UserId == employerId && r.Vacancy != null && r.Vacancy.EmployerId == currentUserId)
            );
        
        bool hasResponse = await _context.Responses
            .Include(r => r.Resume).Include(r => r.Vacancy).AnyAsync(r => 
                (r.Resume != null && r.Resume.UserId == currentUserId && r.Vacancy != null && r.Vacancy.EmployerId == employerId) ||
                (r.Resume != null && r.Resume.UserId == employerId && r.Vacancy != null && r.Vacancy.EmployerId == currentUserId)
            );

        if (!hasResponse)
            return BadRequest("Отправка сообщений возможна только при наличии отклика на вакансию.");

        var message = new Message
        {
            SenderUserId = currentUserId,
            ReceiverUserId = employerId,
            MessageText = messageText.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    
}