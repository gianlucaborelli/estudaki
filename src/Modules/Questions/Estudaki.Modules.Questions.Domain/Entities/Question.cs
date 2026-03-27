using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Entities;

public class Question
{
    public string Id { get; set; } = string.Empty;
    public string? PublicNoticeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = false;
    public bool? IsNullified { get; set; } = false;
    public int QuestionNumber { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public string MainArea { get; set; } = string.Empty;
    public string[] SubAreas { get; set; } = [];
    public List<ContentBlock> QuestionContents { get; set; } = [];
    public List<Choice>? Choices { get; set; }
}
