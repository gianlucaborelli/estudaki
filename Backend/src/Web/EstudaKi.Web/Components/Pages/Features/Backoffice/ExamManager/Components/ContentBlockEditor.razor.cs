using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public class ContentBlockEditorBase : ComponentBase
{
    [Inject] 
    protected ILogger<ContentBlockEditorBase> Logger { get; set; } = default!;

    [Parameter] 
    public List<ContentBlock> ContentBlocks { get; set; } = [];

    [Inject]
    protected IDialogService Dialog { get; set; } = default!;


    [CascadingParameter(Name = "PublicNotice")]
    protected PublicNoticeDto? PublicNotice { get; set; }

    protected enum ContentBlockType { Paragraph, Image }
    
    protected void AddContentBlock(ContentBlockType type)
    {
        var newOrder = ContentBlocks.Any()
            ? ContentBlocks.Max(b => b.Order) + 1
            : 0;

        if (type == ContentBlockType.Paragraph)
        {
            ContentBlocks.Add(new ParagraphBlock
            {
                Order = newOrder,
                Inlines = new List<InlineContent> { new TextInline() },
            });
        }
        else if (type == ContentBlockType.Image)
        {
            ContentBlocks.Add(new ImageBlock
            {
                Order = newOrder,
                Key = $"img-{Guid.NewGuid()}",
                Title = "",
                Source = "",
                Description = ""
            });
        }

        Logger.LogDebug("Bloco de conteúdo adicionado: {Type}", type);
        StateHasChanged();
    }

    protected void ReorderContentBlocks()
    {
        for (int i = 0; i < ContentBlocks.Count; i++)
        {
            ContentBlocks[i].Order = i;
        }
    }

    protected void RemoveContentBlock(int index)
    {
        if (index < 0 || index >= ContentBlocks.Count) return;

        ContentBlocks.RemoveAt(index);
        ReorderContentBlocks();
        Logger.LogDebug("Bloco de conteúdo removido no índice {Index}", index);
        StateHasChanged();
    }

    protected void MoveContentBlockUp(int index)
    {
        if (index <= 0 || index >= ContentBlocks.Count) return;

        var block = ContentBlocks[index];
        ContentBlocks.RemoveAt(index);
        ContentBlocks.Insert(index - 1, block);
        ReorderContentBlocks();
        StateHasChanged();
    }

    protected void MoveContentBlockDown(int index)
    {
        if (index < 0 || index >= ContentBlocks.Count - 1) return;

        var block = ContentBlocks[index];
        ContentBlocks.RemoveAt(index);
        ContentBlocks.Insert(index + 1, block);
        ReorderContentBlocks();
        StateHasChanged();
    }    

    protected async Task OpenImageSelectorForBlock(ContentBlock block)
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
            if (block is ImageBlock imageBlock)
            {
                imageBlock.Key = selectedImageKey;                
                Logger.LogInformation("Imagem {ImageKey} atribuída ao bloco {BlockOrder}", selectedImageKey, block.Order);
                StateHasChanged();
            }
        }
    }
}
