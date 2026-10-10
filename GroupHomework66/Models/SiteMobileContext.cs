using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GroupHomework66.Models;

public class SiteMobileContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Vacancy> Vacancies { get; set; } 
    
    public SiteMobileContext(DbContextOptions<SiteMobileContext> options) : base(options)
    {}
}