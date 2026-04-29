using System.Runtime.CompilerServices;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class PublicNoticeDto
{
    public string Id { get; set; } = string.Empty;
    public string? Number { get; set; }
    public int Year { get; set; }
    public string? ExamPhase { get; set; }
    public string? ExamBoard { get; set; }
    public string? ExamCategory { get; set; }
    public string? Position { get; set; }
    public bool IsReviewed { get; set; }
    public bool IsPublished { get; set; }
    public string? ExamBookletUrl { get; set; }
    public string? AnswerKeyUrl { get; set; }
    public bool HasAttachments { get; set; }

    public DateTime CreatedAt { get; set; }


    public static PublicNoticeDto Clone(PublicNoticeDto original)
    {
        return new PublicNoticeDto
        {
            Id = original.Id,
            Number = original.Number,
            Year = original.Year,
            ExamPhase = original.ExamPhase,
            ExamBoard = original.ExamBoard,
            ExamCategory = original.ExamCategory,
            Position = original.Position,
            IsReviewed = original.IsReviewed,
            IsPublished = original.IsPublished,
            ExamBookletUrl = original.ExamBookletUrl,
            AnswerKeyUrl = original.AnswerKeyUrl,
            HasAttachments = original.HasAttachments,
            CreatedAt = original.CreatedAt
        };
    }
}
