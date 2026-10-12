using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Models;

public class SiteMobileContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Vacancy> Vacancies { get; set; } 
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Course> Courses { get; set; }
    
    public SiteMobileContext(DbContextOptions<SiteMobileContext> options) : base(options)
    {}
}