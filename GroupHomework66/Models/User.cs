using Microsoft.AspNetCore.Identity;

namespace GroupHomework66.Models;

public class User : IdentityUser<int>
{
    public string? ProfilePicture { get; set; }
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}