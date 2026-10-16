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
    public async Task<IActionResult> SendMessageAjax(int candidateId, int employerId, string messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText))
            return BadRequest("Сообщение не может быть пустым.");
        
        var currentUserId = CurrentUserId;

        if (currentUserId != candidateId && currentUserId != employerId)
            return Forbid();

        int receiverId = (currentUserId == candidateId) ? employerId : candidateId;

        if (currentUserId == receiverId)
            return BadRequest("Нельзя отправить сообщение самому себе.");

        bool hasResponse = await _context.Responses
            .AnyAsync(r => r.Resume != null && r.Resume.UserId == candidateId && 
                           r.Vacancy != null && r.Vacancy.EmployerId == employerId);

        if (!hasResponse)
            return BadRequest("Отправка сообщений возможна только при наличии отклика на вакансию.");

        var message = new Message
        {
            SenderUserId = currentUserId,
            ReceiverUserId = receiverId,
            MessageText = messageText.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    
}