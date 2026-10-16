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
    public DbSet<Response> Responses { get; set; }
    public DbSet<Message> Messages { get; set; }
    public SiteMobileContext(DbContextOptions<SiteMobileContext> options) : base(options)
    {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.ReceiverUser)
            .WithMany()
            .HasForeignKey(m => m.ReceiverUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}