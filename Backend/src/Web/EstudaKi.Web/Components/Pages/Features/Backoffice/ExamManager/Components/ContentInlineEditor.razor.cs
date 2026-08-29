using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class ContentInlineEditorBase: ComponentBase
{
    [Inject]
    protected ILogger<ContentInlineEditorBase> Logger { get; set; } = default!;

    [Inject]
    protected IDialogService Dialog { get; set; } = default!;

    [Parameter]
    public List<InlineContent> Inlines { get; set; } = [];

    [CascadingParameter(Name = "PublicNotice")]
    protected PublicNoticeDto? PublicNotice { get; set; }

    protected enum InlineType { Text, Image, Math, Chemical }

    protected override void OnParametersSet()
    {
        Logger.LogDebug("ContentInlineEditor - PublicNotice recebido: {IsNull}, ID: {Id}", 
            PublicNotice == null ? "NULL" : "OK", 
            PublicNotice?.Id ?? "N/A");
    }

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

    protected async Task OpenImageSelectorForInline(int inlineIndex)
    {
        if (PublicNotice == null) return;

        var parameters = new DialogParameters<ImageSelectorModal>
        {
            { c => c.PublicNotice, PublicNotice }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        var dialog = await Dialog.ShowAsync<ImageSelectorModal>("Selecionar Imagem", parameters, options);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is string selectedImageKey)
        {
            if (inlineIndex >= 0 && inlineIndex < Inlines.Count && Inlines[inlineIndex] is ImageInline imageInline)
            {
                imageInline.Key = selectedImageKey;
                Logger.LogInformation("Imagem {ImageKey} atribuída ao inline {InlineIndex}", selectedImageKey, inlineIndex);
                StateHasChanged();
            }
        }
    }
}
