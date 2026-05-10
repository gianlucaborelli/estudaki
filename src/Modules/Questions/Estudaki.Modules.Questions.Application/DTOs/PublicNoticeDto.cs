using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class PublicNoticeDto
{
    public string Id { get; set; } = string.Empty;
    public string? Number { get; set; }
    public int Year { get; set; }
    public string? ExaminerOrganization { get; set; }
    public string? ContractingOrganization { get; set; }
    public string? ExamCategory { get; set; }
    public bool IsReviewed { get; set; }
    public bool IsPublished { get; set; }
    public string? FileUrl { get; set; }
    public List<Exam> Exams { get; set; } = [];
    public int? QuestionCount { get; set; }
    public DateTime CreatedAt { get; set; }


    public static PublicNoticeDto Clone(PublicNoticeDto original)
    {
        return new PublicNoticeDto
        {
            Id = original.Id,
            Number = original.Number,
            Year = original.Year,
            ExaminerOrganization = original.ExaminerOrganization,
            ContractingOrganization = original.
            ExamCategory = original.ExamCategory,
            IsReviewed = original.IsReviewed,
            IsPublished = original.IsPublished,
            FileUrl = original.FileUrl,
            Exams = original.Exams,
            CreatedAt = original.CreatedAt
        };
    }
}
