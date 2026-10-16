namespace GroupHomework66.ViewModels;

public class CompanyResponsesViewModel
{
    public List<VacancyResponsesGroup> Vacancies { get; set; } = new();
}

public class VacancyResponsesGroup
{
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public List<ResponseItem> Responses { get; set; } = new();
}

public class ResponseItem
{
    public int ResponseId { get; set; }
    public int ApplicantId { get; set; }
    public string ResumeTitle { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public DateTime ResponseDate { get; set; }
}