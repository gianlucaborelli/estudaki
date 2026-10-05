namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public class TextInline : InlineContent
{
    public string Text { get; set; } = string.Empty;
    public bool Bold { get; set; }
    public bool Italic { get; set; }

    public TextInline()
    {
        Type = "text";
    }
}
