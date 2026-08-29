namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class Choice
{
    public string? Option { get; set; }

    [Obsolete("Use ContentBlocks property instead.")]
    public List<InlineContent>? Content { get; set; } = [];
    public List<ContentBlock>? ContentBlocks { get; set; } = [];
    public bool IsCorrect { get; set; }
}
