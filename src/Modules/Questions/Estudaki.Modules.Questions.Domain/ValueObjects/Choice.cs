namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class Choice
{
    public string? Option { get; set; }
    public List<InlineContent> Content { get; set; } = [];
    public bool IsCorrect { get; set; }
}
