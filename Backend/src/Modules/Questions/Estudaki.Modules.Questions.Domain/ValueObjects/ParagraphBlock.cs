namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ParagraphBlock : ContentBlock
{
    [Obsolete("Use Text property instead.")]
    public List<InlineContent>? Inlines { get; set; } = null;
    public string? Title { get; set; }
    public string? Source { get; set; }
    public string? Text { get; set; }
}
