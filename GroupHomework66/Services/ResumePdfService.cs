using System.Globalization;
using GroupHomework66.Helpers;
using GroupHomework66.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GroupHomework66.Services;

public class ResumePdfService
{
    public byte[] Generate(Resume resume)
    {
        var ruCulture = CultureInfo.GetCultureInfo("ru-RU");
        var contacts = new List<(string Label, string Value)>
        {
            ("Telegram", resume.Telegram ?? ""),
            ("Email", resume.Email ?? ""),
            ("Телефон", resume.PhoneNumber ?? ""),
            ("Facebook", resume.Facebook ?? ""),
            ("LinkedIn", resume.LinkedIn ?? "")
        };

        contacts = contacts.Where(c => !string.IsNullOrWhiteSpace(c.Value)).ToList();

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(18, Unit.Millimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(style => style.FontSize(10).FontColor(Colors.Grey.Darken4));
                page.Header().Column(header =>
                {
                    header.Spacing(4);
                    header.Item().Text(resume.Title).FontSize(22).Bold().FontColor(Colors.Blue.Darken2);
                    if (!string.IsNullOrWhiteSpace(resume.User?.UserName))
                    {
                        header.Item().Text(resume.User.UserName).FontSize(11).FontColor(Colors.Grey.Darken1);
                    }

                    header.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);
                });

                page.Content().PaddingTop(12).Column(content =>
                {
                    content.Spacing(14);
                    content.Item().Column(section =>
                    {
                        AddSectionHeading(section, "Основная информация");
                        AddLabelValue(section, "Категория", resume.Category.GetDisplayName());
                        AddLabelValue(section, "Ожидаемая зарплата", $"{resume.ExpectedSalary.ToString("N0", ruCulture)} сом");
                        AddLabelValue(section, "Дата обновления", resume.UpdatedDate.ToString("dd.MM.yyyy"));
                    });
                    if (contacts.Count > 0)
                    {
                        content.Item().Column(section =>
                        {
                            AddSectionHeading(section, "Контактная информация");
                            foreach (var contact in contacts)
                            {
                                AddLabelValue(section, contact.Label, contact.Value);
                            }
                        });
                    }
                    if (resume.WorkExperiences.Any())
                    {
                        content.Item().Column(section =>
                        {
                            AddSectionHeading(section, "Опыт работы");
                            foreach (var experience in resume.WorkExperiences.OrderByDescending(x => x.StartDate))
                            {
                                section.Item().PaddingTop(3).Text($"{experience.Position} — {experience.CompanyName}").FontSize(11).Bold();
                                section.Item().Text(FormatPeriod(experience.StartDate, experience.EndDate)).FontSize(9).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrWhiteSpace(experience.Description))
                                {
                                    section.Item().Text(experience.Description);
                                }
                                section.Item().PaddingBottom(4).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                            }
                        });
                    }
                    if (resume.Educations.Any())
                    {
                        content.Item().Column(section =>
                        {
                            AddSectionHeading(section, "Образование");
                            foreach (var education in resume.Educations.OrderByDescending(x => x.StartDate))
                            {
                                section.Item().PaddingTop(3).Text(education.Institution).FontSize(11).Bold();
                                section.Item().Text(education.Specialty);
                                section.Item().Text(FormatPeriod(education.StartDate, education.EndDate)).FontSize(9).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrWhiteSpace(education.Description))
                                {
                                    section.Item().Text(education.Description);
                                }
                                section.Item().PaddingBottom(4).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                            }
                        });
                    }
                    if (resume.Courses.Any())
                    {
                        content.Item().Column(section =>
                        {
                            AddSectionHeading(section, "Курсы и повышение квалификации");

                            foreach (var course in resume.Courses.OrderByDescending(x => x.StartDate))
                            {
                                section.Item().PaddingTop(3).Text(course.Name).FontSize(11).Bold();
                                section.Item().Text(course.Organization);
                                section.Item().Text(FormatPeriod(course.StartDate, course.EndDate)).FontSize(9).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrWhiteSpace(course.Description))
                                {
                                    section.Item().Text(course.Description);
                                }
                                section.Item().PaddingBottom(4).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                            }
                        });
                    }
                });
                page.Footer().PaddingTop(8).AlignCenter().Text(text =>
                    {
                        text.Span("Страница ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                        text.Span(" из ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                    });
            });
        }).GeneratePdf();
    }

    private static void AddSectionHeading(ColumnDescriptor section, string title)
    {
        section.Item().PaddingBottom(4).Text(title).FontSize(13).Bold().FontColor(Colors.Blue.Darken2);
        section.Item().LineHorizontal(0.75f).LineColor(Colors.Grey.Lighten2);
    }

    private static void AddLabelValue(ColumnDescriptor section, string label, string value)
    {
        section.Item().Row(row =>
        {
            row.ConstantItem(125).Text(label).SemiBold();
            row.RelativeItem().Text(value);
        });
    }

    private static string FormatPeriod(DateTime startDate, DateTime? endDate)
    {
        var start = startDate.ToString("MM.yyyy");
        var end = endDate?.ToString("MM.yyyy") ?? "По настоящее время";

        return $"{start} - {end}";
    }
}