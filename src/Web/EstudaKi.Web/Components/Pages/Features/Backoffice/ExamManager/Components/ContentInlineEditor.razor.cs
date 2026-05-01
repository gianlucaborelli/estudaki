using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class ContentInlineEditorBase: ComponentBase
{
    [Inject]
    protected ILogger<ContentInlineEditorBase> Logger { get; set; } = default!;
    [Parameter]
    public List<InlineContent> Inlines { get; set; } = [];

    protected enum InlineType { Text, Image, Math, Chemical }   

    protected void AddInlineContent(InlineType type)
    {
        if (type == InlineType.Text)
        {
            Inlines.Add(new TextInline
            {
                Text = "",
                Bold = false,
                Italic = false
            });
        }
        else if (type == InlineType.Image)
        {
            Inlines.Add(new ImageInline
            {
                Key = $"img-{Guid.NewGuid()}",
                Alt = "",
                Width = 0,
                Height = 0
            });
        }
        else if (type == InlineType.Math)
        {
            Inlines.Add(new MathInline
            {
                Latex = ""
            });
        }
        else if (type == InlineType.Chemical)
        {
            Inlines.Add(new ChemicalFormulaInline
            {
                Formula = ""
            });
        }

        StateHasChanged();
    }

    protected void RemoveInline(int inlineIndex)
    {
        if (inlineIndex < 0 || inlineIndex >= Inlines.Count) return;

        Inlines.RemoveAt(inlineIndex);
        StateHasChanged();
    }

    protected void MoveInlineUp(int inlineIndex)
    {        
        if (inlineIndex <= 0 || inlineIndex >= Inlines.Count) return;

        var inline = Inlines[inlineIndex];
        Inlines.RemoveAt(inlineIndex);
        Inlines.Insert(inlineIndex - 1, inline);
        StateHasChanged();
    }

    protected void MoveInlineDown(int inlineIndex)
    {        
        if (inlineIndex < 0 || inlineIndex >= Inlines.Count - 1) return;

        var inline = Inlines[inlineIndex];
        Inlines.RemoveAt(inlineIndex);
        Inlines.Insert(inlineIndex + 1, inline);
        StateHasChanged();
    }
}
