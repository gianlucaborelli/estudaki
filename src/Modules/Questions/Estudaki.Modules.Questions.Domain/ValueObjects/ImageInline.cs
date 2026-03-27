namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class ImageInline : InlineContent
{
    public string Key { get; set; } = string.Empty;
    public string? Alt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public ImageInline()
    {
        Type = "image";
    }
}
