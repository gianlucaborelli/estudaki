namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class Choice
{
    public string? Option { get; set; }

    [Obsolete("Use Explanation property instead.")]
    public List<InlineContent>? Content { get; set; } = [];
    [Obsolete("Use Explanation property instead.")]
    public List<ContentBlock>? ContentBlocks { get; set; } = [];
    public string? Explanation { get; set; }
    public bool IsCorrect { get; set; }
}
