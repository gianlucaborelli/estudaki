namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ImageBlock : ContentBlock
{
    public string Key { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
}
