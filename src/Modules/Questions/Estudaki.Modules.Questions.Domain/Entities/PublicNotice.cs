using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Questions.Domain.Entities;

public class PublicNotice : Entity
{
    public string? Number { get; set; }
    public int Year { get; set; }
    public string? ExamPhase { get; set; }
    public string? ExamBoard { get; set; }
    public string? Position { get; set; }
    public string? ExamBookletURL { get; set; }
    public string? ExamAnswerKeyURL { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
