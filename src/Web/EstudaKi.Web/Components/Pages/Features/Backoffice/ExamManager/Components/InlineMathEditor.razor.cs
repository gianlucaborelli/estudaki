using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public partial class InlineMathEditorBase : ComponentBase
{
    [Parameter] public string? Latex { get; set; }
    [Parameter] public EventCallback<string> LatexChanged { get; set; }

    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    protected string previewId = $"math-preview-{Guid.NewGuid()}";
    private bool isRendered = false;

    protected override void OnParametersSet()
    {
        _ = UpdatePreview();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            isRendered = true;
            await UpdatePreview();
        }
    }

    protected async Task OnLatexChanged(string newValue)
    {
        if (Latex == newValue)
            return;

        Latex = newValue;
        await LatexChanged.InvokeAsync(Latex);
        await UpdatePreview();
    }

    private async Task UpdatePreview()
    {
        if (!isRendered) return;

        try
        {
            await JSRuntime.InvokeVoidAsync("MathQuillHelper.renderStatic", previewId, Latex);
            await LatexChanged.InvokeAsync(Latex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao renderizar MathQuill: {ex.Message}");
        }
    }
}
