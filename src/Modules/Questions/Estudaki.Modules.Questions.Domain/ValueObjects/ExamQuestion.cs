namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ExamQuestion
{
    public string ExamId { get; set; } = default!;
    public string QuestionId { get; set; } = default!;
    public bool IsNullified { get; set; } = false;
    public int QuestionNumber { get; set; }
}
