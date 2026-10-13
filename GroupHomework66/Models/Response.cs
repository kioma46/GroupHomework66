namespace GroupHomework66.Models;

public class Response
{
    public int Id { get; set; }
    
    public int VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }
    
    public int ResumeId { get; set; }
    public Resume? Resume { get; set; }
    
    public DateTime ResponseDate { get; set; } = DateTime.UtcNow;
}