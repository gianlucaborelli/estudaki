using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class InlineTextEditorBase : ComponentBase, IDisposable
{
    [Inject]
    protected ILogger<InlineTextEditorBase> Logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public TextInline Value { get; set; } = new TextInline();

    private DotNetObjectReference<InlineTextEditorBase>? dotNetRef;

    protected string editorId = $"text-editor-{Guid.NewGuid()}";

    protected string selectedText = string.Empty;
    protected int selectionStart = 0;
    protected int selectionEnd = 0;
    protected bool hasSelection = false;

    protected void OnTextChanged(string newText)
    {
        Value.Text = newText;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("textSelectionTracker.register", editorId, dotNetRef);
        }
    }

    [JSInvokable]
    public void OnSelectionChanged(TextSelection selection)
    {
        selectedText = selection.Text;
        selectionStart = selection.Start;
        selectionEnd = selection.End;
        hasSelection = selection.HasSelection;

        StateHasChanged();
    }

    protected async Task ApplyFormatting(string tag)
    {
        var (openTag, closeTag) = GetFormattingTags(tag);

        if (string.IsNullOrEmpty(openTag))
            return;

        var beforeSelection = Value.Text.Substring(0, selectionStart);
        var afterSelection = Value.Text.Substring(selectionEnd);

        int newCursorPosition;

        if (hasSelection)
        {
            Value.Text = $"{beforeSelection}{openTag}{selectedText}{closeTag}{afterSelection}";
            Logger.LogInformation($"Formatação '{tag}' aplicada à seleção: '{selectedText}'");

            newCursorPosition = beforeSelection.Length + openTag.Length + selectedText.Length + closeTag.Length;
        }
        else
        {
            Value.Text = $"{beforeSelection}{openTag}{closeTag}{afterSelection}";
            Logger.LogInformation($"Tags '{tag}' inseridas na posição do cursor: {selectionStart}");

            newCursorPosition = beforeSelection.Length + openTag.Length;
        }

        StateHasChanged();

        await Task.Delay(50);
        await JSRuntime.InvokeVoidAsync("textSelectionTracker.setCursorPosition", editorId, newCursorPosition);
    }

    private (string openTag, string closeTag) GetFormattingTags(string tag)
    {
        return tag switch
        {
            "bold" => ("<strong>", "</strong>"),
            "italic" => ("<em>", "</em>"),
            "underline" => ("<u>", "</u>"),
            "math" => ("<math>", "</math>"),
            "chemical" => ("<chemical>", "</chemical>"),
            "inline-image" => ("<inline-image>", "</inline-image>"),
            _ => ("", "")
        };
    }

    public void Dispose()
    {
        if (dotNetRef != null)
        {
            JSRuntime.InvokeVoidAsync("textSelectionTracker.unregister", editorId);
            dotNetRef.Dispose();
        }
    }

    public class TextSelection
    {
        public string Text { get; set; } = string.Empty;
        public int Start { get; set; }
        public int End { get; set; }
        public bool HasSelection { get; set; }
    }
}
