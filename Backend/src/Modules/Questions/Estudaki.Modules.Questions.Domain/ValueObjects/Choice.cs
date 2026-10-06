namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class Choice
{
    public string? Option { get; set; }    
    public string? Explanation { get; set; }
    public bool IsCorrect { get; set; }
}
