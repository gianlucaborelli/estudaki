using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class QuestionDto
{
    public string Id { get; set; } = string.Empty;
    public string? PublicNoticeId { get; set; }
    public PublicNoticeDto? PublicNotice { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool? IsNullified { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public string MainArea { get; set; } = string.Empty;
    public string[] SubAreas { get; set; } = [];
    public List<ContentBlock> QuestionContents { get; set; } = [];
    public List<Choice>? Choices { get; set; }
}
