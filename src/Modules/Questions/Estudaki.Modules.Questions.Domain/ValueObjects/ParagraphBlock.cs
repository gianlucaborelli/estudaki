namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ParagraphBlock : ContentBlock
{
    public List<InlineContent> Inlines { get; set; } = [];
    public string? Title { get; set; }
    public string? Source { get; set; }
}
